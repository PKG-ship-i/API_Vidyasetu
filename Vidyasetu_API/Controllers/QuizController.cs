using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vidyasetu_API.Common;
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
		[Authorize]
		public async Task<IActionResult> ShareQuiz([FromBody] ShareQuizRequest request)
		{
			// For now, simulate current user (in hackathon, assume userId = 1)
			//long currentUserId = 1;
			long currentUserId = long.Parse(User.FindFirst("userId")?.Value ?? "2");

			string shareCode = await _quizService.ShareQuizAsync(request, currentUserId);
			// ✅ Build dynamic URL
			string baseUrl = $"{Request.Scheme}://{Request.Host}";
			string shareUrl = $"{baseUrl}/quiz/{shareCode}";

			var response = new
			{
				shareCode,
				shareUrl
			};

			return Ok(ApiResponse<object>.CreateSuccess(response, "Quiz shared successfully"));
		}

		[HttpPost("{shareCode}/join")]
		public async Task<IActionResult> JoinQuiz(string shareCode, [FromBody] JoinQuizRequest request)
		{
			try
			{
				var participantId = await _quizService.CreateParticipantAsync(shareCode, request);
				var quiz = await _quizService.GetQuizByShareCodeAsync(shareCode);

				var response = new
				{
					participantId,
					quiz
				};

				return Ok(ApiResponse<object>.CreateSuccess(response, "Participant joined successfully"));
			}
			catch (Exception ex)
			{
				return BadRequest(ApiResponse<object>.CreateFailure(ex.Message));
			}
		}


		// POST: /api/quiz/{shareCode}/submit
		[HttpPost("{shareCode}/submit")]
		public async Task<IActionResult> SubmitAnswers(string shareCode, [FromBody] SubmitQuizAnswersRequest request)
		{
			try
			{
				await _quizService.SubmitAnswersAsync(shareCode, request);
				return Ok(ApiResponse<string>.CreateSuccess("Answers submitted successfully!"));
			}
			catch (Exception ex)
			{
				return BadRequest(ApiResponse<string>.CreateFailure(ex.Message));
			}
		}

		// GET: /api/quiz/{shareCode}/leaderboard
		[HttpGet("{shareCode}/leaderboard")]
		public async Task<IActionResult> GetLeaderboard(string shareCode)
		{
			try
			{
				var leaderboard = await _quizService.GetLeaderboardAsync(shareCode);
				return Ok(ApiResponse<List<LeaderboardEntryDto>>.CreateSuccess(leaderboard, "Leaderboard fetched"));
			}
			catch (Exception ex)
			{
				return BadRequest(ApiResponse<string>.CreateFailure(ex.Message));
			}
		}
	}
}
