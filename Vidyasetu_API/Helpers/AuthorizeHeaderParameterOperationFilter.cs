using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vidyasetu_API.Helpers
{
	public class AuthorizeHeaderParameterOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Security == null || operation.Security.Count == 0)
				return;

			foreach (var param in operation.Parameters ?? Enumerable.Empty<OpenApiParameter>())
			{
				if (param.Name == "Authorization")
				{
					param.Description += " (Automatically adds 'Bearer ' prefix)";
				}
			}
		}
	}
}
