using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Vidyasetu_API.DTOs.Response
{
    public class QuestionnaireResponseModel
    {

        public List<Question>? Questions { get; set; } = default!;
        public string Summary { get; set; } = default!;
        public List<Flashcard>? Flashcards { get; set; } = default!;


    }

    public class Question
    {
		[JsonProperty("id")] // for Newtonsoft.Json
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("question_text")]
        public string QuestionText { get; set; } = default!;

        public List<string> Options { get; set; } = default!;

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = default!;

        public string Explanation { get; set; } = default!;

        public string Timestamp { get; set; } = default!; // Can be changed to double if needed
    }

    public class Flashcard
    {
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
    }


}
