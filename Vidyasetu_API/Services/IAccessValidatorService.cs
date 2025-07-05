using Vidyasetu_API.Common;
using Vidyasetu_API.Models;

namespace Vidyasetu_API.Services
{
	public interface IAccessValidatorService
	{
		Task<ApiResponse<string>?> ValidateTrialLimitAsync(DeviceDetail device, int logCount);
	}
}
