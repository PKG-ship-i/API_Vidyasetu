using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vidyasetu_API.DTOs.Request;
using Vidyasetu_API.DTOs.Response;
using Vidyasetu_API.Hubs;
using Vidyasetu_API.Models;

namespace Vidyasetu_API.Services
{
	public class QuizService : IQuizService
	{
		private readonly IHubContext<LeaderboardHub> _hubContext;
		private readonly VidyasetuAI_DevContext _db;

		public QuizService(IHubContext<LeaderboardHub> hubContext, VidyasetuAI_DevContext db)
		{
			_hubContext = hubContext;
			_db = db;
		}

		public async Task<string> ShareQuizAsync(ShareQuizRequest request, long userId)
		{
			string shareCode = GenerateCode(); // e.g., random 6-digit code
											   // Save in DB `shared_quiz_sessions` (EF or Dapper)

			return shareCode;
		}

		public async Task SubmitAnswersAsync(string shareCode, SubmitQuizAnswersRequest request)
		{
			// 1. Load sessionId from shareCode
			// 2. Validate + store participant and answers
			// 3. Calculate score
			// 4. Save to `quiz_participants` and `participant_answers`
			// 5. Fetch updated leaderboard
			var leaderboard = await GetLeaderboardAsync(shareCode);

			// 6. Push update via SignalR
			await _hubContext.Clients.Group(shareCode)
				.SendAsync("UpdateLeaderboard", leaderboard);
		}

		public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(string shareCode)
		{
			// Query DB: Load participants from `quiz_participants`
			return new List<LeaderboardEntryDto>();
		}

		private string GenerateCode()
		{
			return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
		}

		public async Task<long> CreateParticipantAsync(string shareCode, JoinQuizRequest request)
		{
			var session = await _db.SharedQuizSessions
				.FirstOrDefaultAsync(x => x.ShareCode == shareCode && x.ActiveFlag == true);

			if (session == null)
				throw new Exception("Invalid or inactive quiz link.");

			// Check if nickname is already taken for this quiz
			bool nicknameExists = await _db.QuizParticipants
				.AnyAsync(p => p.SessionId == session.Id && p.Nickname == request.Nickname);

			if (nicknameExists)
				throw new Exception("Nickname already taken for this quiz.");

			var participant = new QuizParticipant
			{
				SessionId = session.Id,
				Nickname = request.Nickname,
				ActiveFlag = true,
				SubmittedAt = null,
				DeviceId = request.DeviceId
			};

			_db.QuizParticipants.Add(participant);
			await _db.SaveChangesAsync();

			return participant.Id;
		}

		public async Task<QuestionnaireResponseModel> GetQuizByShareCodeAsync(string shareCode)
		{
			var session = await _db.SharedQuizSessions
				.FirstOrDefaultAsync(x => x.ShareCode == shareCode && x.ActiveFlag == true);

			if (session == null)
				throw new Exception("Invalid quiz link.");

			var quizData = await _db.UserRequestResponses
				.Where(x => x.RequestId == session.RequestId)
				.Select(x => x.QuestionJson)
				.FirstOrDefaultAsync();

			if (string.IsNullOrEmpty(quizData))
				throw new Exception("Quiz content not found.");

			return JsonConvert.DeserializeObject<QuestionnaireResponseModel>(quizData)
				?? throw new Exception("Invalid quiz format.");
		}


	}
}
