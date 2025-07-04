using System.Text.Json.Serialization;

namespace Vidyasetu_API.Models
{
    public class QuestionModel
    {
        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; } = string.Empty;

        [JsonPropertyName("options")]
        public List<string> Options { get; set; } = new();

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}
