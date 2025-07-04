using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using Vidyasetu_API.DTOs.Response;
using Vidyasetu_API.Models;

namespace Vidyasetu_API.Common
{
    public class HelperService
    {
        private readonly VidyasetuAI_DevContext _db;
        private readonly IConfiguration _config;

        public HelperService(VidyasetuAI_DevContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<DeviceDetail?> IsDeviceAllowedAsync(long deviceId)
        {
            // Step 1: Check if device has >= 3 logs

            // Step 2: Get the device and its associated user
            return await _db.DeviceDetails
                .Where(d => d.Id == deviceId)
                .FirstOrDefaultAsync();

          
        }

        public async Task<DeviceLogDetail> AddNewDevicelog(DeviceLogDetail deviceLogDetail)
        {
            await _db.DeviceLogDetails.AddAsync(deviceLogDetail);
            await _db.SaveChangesAsync();
            return deviceLogDetail;
        }



        public async Task<UserRequestPreference> AddUserPreference(UserRequestPreference userRequestPreference)
        {
            await _db.UserRequestPreferences.AddAsync(userRequestPreference);
            await _db.SaveChangesAsync();
            return userRequestPreference;
        }
        public async Task<GeneratedQuestionResponse?> IsRequestExist(
            int numberOfQuestion,
            int sourceTypeId,
            int diffcultyTypeId,
            int questionTypeId,
            string sourceUrl)
        {
            var response = new QuestionnaireResponseModel();

            // Get the matching UserRequestPreference ID based on DeviceLogDetails filter
            var matchingId = await _db.DeviceLogDetails
                .Where(x => x.RequestUrl == sourceUrl && x.SourceTypeId == sourceTypeId)
                .SelectMany(x => x.UserRequestPreferences)
                .Where(y => y.NumberOfQuestions == numberOfQuestion
                         && y.DifficultyTypeId == diffcultyTypeId
                         && y.QuestionsTypeId == questionTypeId)
                .Select(x => x.RequestId)
                .FirstOrDefaultAsync();

            if (matchingId == 0)
                return null;

            // Get existing user request response based on RequestId
            var existingRequest = await _db.UserRequestResponses
                .FirstOrDefaultAsync(x => x.RequestId == matchingId);

            if (existingRequest == null)
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize Questions if available
            if (!string.IsNullOrWhiteSpace(existingRequest.QuestionJson))
            {
                response.Questions = JsonSerializer.Deserialize<List<Question>>(existingRequest.QuestionJson, options);
            }

            // Set Summary string directly
            if (!string.IsNullOrWhiteSpace(existingRequest.SummaryJson))
            {
                response.Summary = existingRequest.SummaryJson;
            }

            // Deserialize Flashcards if available
            if (!string.IsNullOrWhiteSpace(existingRequest.FlashcardJson))
            {
                response.Flashcards = JsonSerializer.Deserialize<List<Flashcard>>(existingRequest.FlashcardJson, options);
            }


			var logdetails = await _db.DeviceLogDetails.FirstOrDefaultAsync(x=>x.Id == matchingId);
			var device = await _db.DeviceDetails
                .Where(x => x.Id == logdetails!.DeviceId)
                .Select(x => new { x.DeviceIdentifier, x.DeviceToken, x.Id })
                .FirstOrDefaultAsync();

            var result = new GeneratedQuestionResponse()
            {
                token = EncryptDecryptHelper.Encrypt(existingRequest.Id.ToString(), device!.Id.ToString()),
                questionnaireResponseModel = response
            };

            return result;
        }


        public string GetDescriptionFromValue<TEnum>(int value) where TEnum : Enum
        {
            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            var memberInfo = typeof(TEnum).GetMember(enumValue.ToString());

            if (memberInfo.Length > 0)
            {
                var attr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description ?? enumValue.ToString();
            }

            return enumValue.ToString();
        }




    }
}
