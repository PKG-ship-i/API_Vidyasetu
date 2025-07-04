using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vidyasetu_API.DTOs.Request;
using Vidyasetu_API.Services;

namespace Vidyasetu_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuizController : ControllerBase
	{
		private readonly IQuizService _quizService;

		public QuizController(IQuizService quizService)
		{
			_quizService = quizService;
		}

		// POST: /api/quiz/share
		[HttpPost("share")]
		public async Task<IActionResult> ShareQuiz([FromBody] ShareQuizRequest request)
		{
			// For now, simulate current user (in hackathon, assume userId = 1)
			long currentUserId = 1;

			string shareCode = await _quizService.ShareQuizAsync(request, currentUserId);
			return Ok(new
			{
				shareCode,
				shareUrl = $"https://dronaops.com/quiz/{shareCode}"
			});
		}

		[HttpPost("{shareCode}/join")]
		public async Task<IActionResult> JoinQuiz(string shareCode, [FromBody] JoinQuizRequest request)
		{
			var participantId = await _quizService.CreateParticipantAsync(shareCode, request);
			var quiz = await _quizService.GetQuizByShareCodeAsync(shareCode);

			return Ok(new
			{
				participantId,
				quiz
			});
		}


		// POST: /api/quiz/{shareCode}/submit
		[HttpPost("{shareCode}/submit")]
		public async Task<IActionResult> SubmitAnswers(string shareCode, [FromBody] SubmitQuizAnswersRequest request)
		{
			await _quizService.SubmitAnswersAsync(shareCode, request);
			return Ok(new { message = "Answers submitted successfully!" });
		}

		// GET: /api/quiz/{shareCode}/leaderboard
		[HttpGet("{shareCode}/leaderboard")]
		public async Task<IActionResult> GetLeaderboard(string shareCode)
		{
			var leaderboard = await _quizService.GetLeaderboardAsync(shareCode);
			return Ok(leaderboard);
		}
	}
}
