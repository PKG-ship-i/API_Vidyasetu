using Microsoft.OpenApi.Models;
using Vidyasetu_API.Helpers;

namespace Vidyasetu_API.Extensions
{
	public static class SwaggerServiceExtensions
	{
		public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					Description = "Paste your JWT token only. 'Bearer' prefix will be added automatically."
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});

				// 🔹 This tells Swagger to apply the operation filter
				options.OperationFilter<AuthorizeHeaderParameterOperationFilter>();
			});

			return services;
		}
	}
}
