using Vidyasetu_API.DTOs.Response;

namespace Vidyasetu_API.Models
{
    public class GeneratedQuestionResponse
    {
        public string token { get; set; } = string.Empty;
        public QuestionnaireResponseModel questionnaireResponseModel { get; set; } = new QuestionnaireResponseModel();
    }
}
