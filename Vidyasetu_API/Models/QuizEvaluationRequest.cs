namespace Vidyasetu_API.Models
{
    public class QuizEvaluationRequest
    {
        public string token { get; set; } = string.Empty;
        public string TotalTimeTaken { get; set; } = string.Empty;

        public List<UserAnswerModel> Answers { get; set; } = new();
    }

    public class UserAnswerModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string GivenAnswer { get; set; } = string.Empty;
    }
}
