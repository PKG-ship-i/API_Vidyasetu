namespace Vidyasetu_API.DTOs.Request
{
	public class SubmitQuizAnswersRequest
	{
		public string Nickname { get; set; } = default!;
		public long ParticipantId { get; set; }
		public List<SubmittedAnswer> Answers { get; set; } = new();
	}

	public class SubmittedAnswer
	{
		public string QuestionId { get; set; } = default!;
		public string SelectedOption { get; set; } = default!;
	}
}
