using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vidyasetu_API.Common;
using Vidyasetu_API.Models;
using Vidyasetu_API.DTOs;
using Vidyasetu_API.DTOs.Response;
using Azure.Core;

namespace VidyasetuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateResponseController : ControllerBase
    {
        private readonly VidyasetuAI_DevContext _db;
        private readonly HelperService _helperService;
        private readonly IConfiguration _config;


        public GenerateResponseController(VidyasetuAI_DevContext db, HelperService helperService, IConfiguration config)
        {
            _db = db;
            _helperService = helperService;
            _config = config;
        }



		#region Generte Response From video url 


		[HttpPost("GenrateQuestionnaireFromVideoURL")]
		public async Task<IActionResult> GenrateQuestionnaireFromVideoURL([FromBody] GenerateVideoRequestModel dto)
		{
			try
			{
				if (!await _helperService.IsDeviceAllowedAsync(dto.DeviceId))
					return Unauthorized(ApiResponse<string>.CreateFailure("Unauthorized device", 401));

				if (!dto.SourceTypeId.Equals((int)SourceType.Youtube))
					return BadRequest(ApiResponse<string>.CreateFailure("Only YouTube source is supported", 400));

				var addedLog = await _helperService.AddNewDevicelog(new DeviceLogDetail
				{
					DeviceId = dto.DeviceId,
					SourceTypeId = dto.SourceTypeId,
					RequestUrl = dto.VideoUrl,
					ActiveFlag = true,
					CreatedDate = DateTime.UtcNow,
					CreatedBy = 0
				});

				var createQuizRequest = new GenerateQuizRequest
				{
					SourceType = _helperService.GetDescriptionFromValue<SourceType>(dto.SourceTypeId),
					Source = dto.VideoUrl,
					NumQuestions = dto.NumberOfQuestions,
					Difficulty = _helperService.GetDescriptionFromValue<DifficultyLevel>(dto.DifficultyTypeId),
					PreviousQuestions = [],
					QuizLanguage = _helperService.GetDescriptionFromValue<LanguageType>(dto.LanguageId)
				};

				var result = await GenerateQuizAsync(createQuizRequest, addedLog.Id);

                var response = new GeneratedQuestionResponse()
                {
                    token = EncryptDecryptHelper.Encrypt(addedLog.Id.ToString(), dto.DeviceId.ToString()),
                    questionnaireResponseModel = result
                };


                return Ok(ApiResponse<GeneratedQuestionResponse>.CreateSuccess(response, "Questionnaire generated successfully"));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GenrateQuestionnaireFromVideoURL: {ex.Message}");
				return StatusCode(500, ApiResponse<string>.CreateFailure("Internal server error"));
			}
		}




        [HttpPost("GenrateQuestionnaireFromContext")]
        public async Task<QuestionnaireResponseModel?> GenrateQuestionnaireFromContext([FromBody] GenerateContextRequestModel dto)
        {
            try
            {
                var responnse = new QuestionnaireResponseModel();
                var IsvalidDevice = await _helperService.IsDeviceAllowedAsync(dto.DeviceId);

                if (IsvalidDevice && dto.SourceTypeId.Equals(SourceType.Context))
                {

                    var addedLog = await _helperService.AddNewDevicelog(new DeviceLogDetail
                    {
                        DeviceId = dto.DeviceId,
                        SourceTypeId = dto.SourceTypeId,
                        //RequestUrl = dto.VideoUrl,
                        ActiveFlag = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 0 // Assuming 0 for system user, adjust as needed
                    });

                    var createQuizRequest = new GenerateQuizRequest
                    {
                        SourceType = _helperService.GetDescriptionFromValue<SourceType>(dto.SourceTypeId),
                        Source = dto.Context,
                        NumQuestions = dto.NumberOfQuestions,
                        Difficulty = _helperService.GetDescriptionFromValue<DifficultyLevel>(dto.DifficultyTypeId),
                        PreviousQuestions = [],
                        QuizLanguage = _helperService.GetDescriptionFromValue<LanguageType>(dto.LanguageId)

                        //QuestionTypeId = dto.QuestionsTypeId!,
                    };
                    responnse = await GenerateQuizAsync(createQuizRequest, addedLog.Id);
                }

                return responnse;
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error in GenrateQuestionnaireFromVideoURL: {ex.Message}");
                throw; // Re-throw the exception to be handled by global exception handler
            }

        }



        [HttpPost("GenrateQuestionnaireFromPrompt")]
        public async Task<QuestionnaireResponseModel?> GenrateQuestionnaireFromPrompt([FromBody] GeneratePromptRequestModel dto)
        {
            try
            {
                var responnse = new QuestionnaireResponseModel();
                var IsvalidDevice = await _helperService.IsDeviceAllowedAsync(dto.DeviceId);

                if (IsvalidDevice && dto.SourceTypeId.Equals(SourceType.Prompt))
                {

                    var addedLog = await _helperService.AddNewDevicelog(new DeviceLogDetail
                    {
                        DeviceId = dto.DeviceId,
                        SourceTypeId = dto.SourceTypeId,
                        //RequestUrl = dto.VideoUrl,
                        ActiveFlag = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 0 // Assuming 0 for system user, adjust as needed
                    });

                    var createQuizRequest = new GenerateQuizRequest
                    {
                        SourceType = _helperService.GetDescriptionFromValue<SourceType>(dto.SourceTypeId),
                        Source = dto.Prompt,
                        NumQuestions = dto.NumberOfQuestions,
                        Difficulty = _helperService.GetDescriptionFromValue<DifficultyLevel>(dto.DifficultyTypeId),
                        PreviousQuestions = [],
                        QuizLanguage = _helperService.GetDescriptionFromValue<LanguageType>(dto.LanguageId)

                        //QuestionTypeId = dto.QuestionsTypeId!,
                    };
                    responnse = await GenerateQuizAsync(createQuizRequest, addedLog.Id);
                }

                return responnse;
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error in GenrateQuestionnaireFromVideoURL: {ex.Message}");
                throw; // Re-throw the exception to be handled by global exception handler
            }

        }


		#endregion

		#region Helper 
		private async Task<QuestionnaireResponseModel?> GenerateQuizAsync(GenerateQuizRequest requestModel, long RequestId)
        {
            using var client = new HttpClient();

            var url = _config["PythonURL"];

            var json = JsonSerializer.Serialize(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

            var parsed = JsonSerializer.Deserialize<QuizResponseModel>(responseBody);

            var responseEntity = new UserRequestResponse
            {
                RequestId = RequestId,
                QuestionJson = JsonSerializer.Serialize(parsed?.Questions),
                FlashcardJson = JsonSerializer.Serialize(parsed?.Flashcards),
                SummaryJson = JsonSerializer.Serialize(parsed?.Summary)
            };

            _db.UserRequestResponses.Add(responseEntity);
            await _db.SaveChangesAsync();
            /// Need to insert into the database for QuestionRequetResponse

            var result = JsonSerializer.Deserialize<QuestionnaireResponseModel>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result;
        }
        #endregion

    }
}
