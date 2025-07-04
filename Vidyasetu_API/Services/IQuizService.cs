using Vidyasetu_API.DTOs.Request;
using Vidyasetu_API.DTOs.Response;

namespace Vidyasetu_API.Services
{
	public interface IQuizService
	{
		Task<string> ShareQuizAsync(ShareQuizRequest request, long userId);
		Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(string shareCode);
		Task SubmitAnswersAsync(string shareCode, SubmitQuizAnswersRequest request);
		Task<QuestionnaireResponseModel> GetQuizByShareCodeAsync(string shareCode);
		Task<long> CreateParticipantAsync(string shareCode, JoinQuizRequest request);
		Task SubmitLiveAnswersAsync(string shareCode, SubmitQuizAnswersRequest request);
	}
}
