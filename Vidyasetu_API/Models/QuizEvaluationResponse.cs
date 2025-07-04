namespace Vidyasetu_API.Models
{
    public class QuizEvaluationResponse
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
        public string Result { get; set; } = string.Empty; // "Pass" or "Fail"
        public string Medal { get; set; } = string.Empty; // "Gold", "Silver", "Bronze", etc.

        public List<IncorrectQuestionDetail> IncorrectQuestions { get; set; } = new();
    }

    public class IncorrectQuestionDetail
    {
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string GivenAnswer { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
    }
}
