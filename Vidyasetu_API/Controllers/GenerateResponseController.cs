using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Vidyasetu_API.Common;
using Vidyasetu_API.DTOs;
using Vidyasetu_API.DTOs.Response;
using Vidyasetu_API.Models;

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
                var device = await _helperService.IsDeviceAllowedAsync(dto.DeviceId);

                  if (device == null)
                    return Unauthorized(ApiResponse<string>.CreateFailure("Unauthorized device or", 401));
                    int logCount = await _db.DeviceLogDetails.CountAsync(log => log.DeviceId == device.Id);


                if (logCount >= Convert.ToInt32(_config["AllowedRequestCount"]))
                    return StatusCode(302, ApiResponse<string>.CreateFailure("you have exceded the limit of free access, please do login or signup for further process"));

                if (!dto.SourceTypeId.Equals((int)SourceType.Youtube))
                    return BadRequest(ApiResponse<string>.CreateFailure("Only YouTube source is supported", 400));


                // check for same existing Request

                var checkExistingRequest = await _helperService.IsRequestExist(
                    dto.NumberOfQuestions,
                    dto.SourceTypeId,
                    dto.DifficultyTypeId,
                    dto.QuestionsTypeId,
                    dto.VideoUrl);
                if (checkExistingRequest == null)
                {
                    var addedLog = await _helperService.AddNewDevicelog(new DeviceLogDetail
                    {
                        DeviceId = dto.DeviceId,
                        SourceTypeId = dto.SourceTypeId,
                        RequestUrl = dto.VideoUrl,
                        ActiveFlag = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 0
                    });


                    var userRequestPreference = new UserRequestPreference
                    {
                        RequestId = addedLog.Id,
                        NumberOfQuestions = dto.NumberOfQuestions,
                        DifficultyTypeId = dto.DifficultyTypeId,
                        QuestionsTypeId = dto.QuestionsTypeId,
                        LanguageId = dto.LanguageId,
                    };

                    await _helperService.AddUserPreference(userRequestPreference);

                    var createQuizRequest = new GenerateQuizRequest
                    {
                        SourceType = _helperService.GetDescriptionFromValue<SourceType>(dto.SourceTypeId),
                        Source = dto.VideoUrl,
                        NumQuestions = dto.NumberOfQuestions,
                        QuestionType = _helperService.GetDescriptionFromValue<QuestionType>(dto.QuestionsTypeId),
                        Difficulty = _helperService.GetDescriptionFromValue<DifficultyLevel>(dto.DifficultyTypeId),
                        PreviousQuestions = [],
                        QuizLanguage = _helperService.GetDescriptionFromValue<LanguageType>(dto.LanguageId)
                    };

                    var result = await GenerateQuizAsync(createQuizRequest, addedLog.Id);
                    //return Ok(ApiResponse<QuestionnaireResponseModel>.CreateSuccess(result!, "Questionnaire generated successfully"));

                    var response = new GeneratedQuestionResponse()
                    {
                        token = EncryptDecryptHelper.Encrypt(addedLog.Id.ToString(), dto.DeviceId.ToString()),
                        questionnaireResponseModel = result!
                    };


                    return Ok(ApiResponse<GeneratedQuestionResponse>.CreateSuccess(response, "Questionnaire generated successfully"));
                }

               
                return StatusCode(300, ApiResponse<GeneratedQuestionResponse>.CreateSuccess(checkExistingRequest, "Found existing questionnaire and regenerated successfully"));
            }
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GenrateQuestionnaireFromVideoURL: {ex.Message}");
				return StatusCode(500, ApiResponse<string>.CreateFailure("Internal server error"));
			}
		}



        [HttpPost("GenrateQuestionnaireFromPrompt")]
        public async Task<IActionResult?> GenrateQuestionnaireFromPrompt([FromBody] GeneratePromptRequestModel dto)
        {
            try
            {
                var device = await _helperService.IsDeviceAllowedAsync(dto.DeviceId);

                  if (device == null)
                    return Unauthorized(ApiResponse<string>.CreateFailure("Unauthorized device or", 401));
                int logCount = await _db.DeviceLogDetails.CountAsync(log => log.DeviceId == device.Id);


                if (logCount >= Convert.ToInt32(_config["AllowedRequestCount"]))
                    return StatusCode(302, ApiResponse<string>.CreateFailure("you have exceded the limit of free access, please do login or signup for further process"));

                if (!dto.SourceTypeId.Equals((int)SourceType.Youtube))
                    return BadRequest(ApiResponse<string>.CreateFailure("Only YouTube source is supported", 400));



                if (!dto.SourceTypeId.Equals((int)SourceType.Prompt))
                    return BadRequest(ApiResponse<string>.CreateFailure("Only Prompt source is supported", 400));


                    var addedLog = await _helperService.AddNewDevicelog(new DeviceLogDetail
                    {
                        DeviceId = dto.DeviceId,
                        SourceTypeId = dto.SourceTypeId,
                        RequestUrl = dto.Prompt,
                        ActiveFlag = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 0
                    });


                    var userRequestPreference = new UserRequestPreference
                    {
                        RequestId = addedLog.Id,
                        NumberOfQuestions = dto.NumberOfQuestions,
                        DifficultyTypeId = dto.DifficultyTypeId,
                        QuestionsTypeId = dto.QuestionsTypeId,
                        LanguageId = dto.LanguageId,
                    };

                    await _helperService.AddUserPreference(userRequestPreference);

                    var createQuizRequest = new GenerateQuizRequest
                    {
                        SourceType = _helperService.GetDescriptionFromValue<SourceType>(dto.SourceTypeId),
                        Source = dto.Prompt,
                        NumQuestions = dto.NumberOfQuestions,
                        Difficulty = _helperService.GetDescriptionFromValue<DifficultyLevel>(dto.DifficultyTypeId),
                        PreviousQuestions = [],
                        QuizLanguage = _helperService.GetDescriptionFromValue<LanguageType>(dto.LanguageId),
                        QuestionType = _helperService.GetDescriptionFromValue<QuestionType>(dto.QuestionsTypeId),
                    };

                var result = await GenerateQuizAsync(createQuizRequest, addedLog.Id);
                var response = new GeneratedQuestionResponse()
                {
                    token = EncryptDecryptHelper.Encrypt(addedLog.Id.ToString(), dto.DeviceId.ToString()),
                    questionnaireResponseModel = result!
                };
                return Ok(ApiResponse<GeneratedQuestionResponse>.CreateSuccess(response!, "Questionnaire generated successfully"));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.CreateFailure("Irrelvant content or server error"));

            }

        }


		#endregion

		#region Helper 
		private async Task<QuestionnaireResponseModel?> GenerateQuizAsync(GenerateQuizRequest requestModel, long RequestId)
        {
            try
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

            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateQuizAsync: {ex.Message}");
                throw; // Re-throw the exception to be handled by global exception handler
            }
           
        }
        #endregion

    }
}
