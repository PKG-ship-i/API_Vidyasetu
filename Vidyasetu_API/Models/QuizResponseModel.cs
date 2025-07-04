using System.Text.Json.Serialization;

namespace Vidyasetu_API.Models
{
    public class QuizResponseModel
    {
        [JsonPropertyName("questions")]
        public List<object>? Questions { get; set; }

        [JsonPropertyName("flashcards")]
        public List<object>? Flashcards { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("recommendations")]
        public List<object>? Recommendations { get; set; }
    }
}
