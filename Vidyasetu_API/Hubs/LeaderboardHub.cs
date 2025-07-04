using Microsoft.AspNetCore.SignalR;

namespace Vidyasetu_API.Hubs
{
	public class LeaderboardHub : Hub
	{
		public async Task JoinQuizRoom(string shareCode)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, shareCode);
		}

		public async Task LeaveQuizRoom(string shareCode)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, shareCode);
		}
	}
}
