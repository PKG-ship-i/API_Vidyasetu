using Vidyasetu_API.DTOs.Response;

namespace Vidyasetu_API.Models
{
    public class QuizEvaluationResponse
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
        public string Result { get; set; } = string.Empty; // "Pass" or "Fail"
        public string Medal { get; set; } = string.Empty; // "Gold", "Silver", "Bronze", etc.
        public string VideoUrl { get; set; } = string.Empty;
        public string TotalTimeTaken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = default!;
        public List<Flashcard>? Flashcards { get; set; } = default!;
        public List<IncorrectQuestionDetail> IncorrectQuestions { get; set; } = new();
        public List<Recommendation>? Recommendations { get; set; } = default!;

        public List<CorrrectQuestionDetail> CorrectQuestions { get; set; } = new();
    }

    public class IncorrectQuestionDetail
    {
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string GivenAnswer { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
    }

    public class CorrrectQuestionDetail
    {
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string GivenAnswer { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
    }
}
