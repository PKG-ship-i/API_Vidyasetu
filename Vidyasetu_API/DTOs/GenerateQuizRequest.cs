using System.Text.Json.Serialization;

namespace Vidyasetu_API.DTOs
{
    //public class GenerateQuizRequest
    //{

    //    public int SourceTypeId { get; set; } = default!;
    //    public string Source { get; set; } = default!;
    //    public int NumQuestions { get; set; } = 5; // Default value
    //    public int DifficultyTypeId { get; set; } = default!;
    //    public string PreviousQuestions { get; set; } = default!;
    //    public int LanguageId { get; set; } = default!;
    //    public int QuestionTypeId { get; set; } = default!; // e.g., "MCQ", "True/False", etc.
    //}

    public class GenerateQuizRequest
    {
        [JsonPropertyName("source_type")]
        public string SourceType { get; set; } = default!;

        [JsonPropertyName("source")]
        public string Source { get; set; } = default!;

        [JsonPropertyName("question_type")]
        public string QuestionType { get; set; } = default!;

        [JsonPropertyName("num_questions")]
        public int NumQuestions { get; set; }

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = default!;

        [JsonPropertyName("previous_questions")]
        public List<string> PreviousQuestions { get; set; } = new();

        [JsonPropertyName("quiz_language")]
        public string QuizLanguage { get; set; } = default!;
    }
}
