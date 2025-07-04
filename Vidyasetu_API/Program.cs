using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vidyasetu_API;
using Vidyasetu_API.Common;
using Vidyasetu_API.Extensions;
using Vidyasetu_API.Models;
using Vidyasetu_API.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Add CORS with specific frontend origin
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("http://localhost:5173") // 👈 Your Vite frontend
			  .AllowAnyHeader()
			  .AllowAnyMethod();
		//.AllowCredentials(); // Optional if using cookies
	});
});

// ✅ 2. JWT Config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtIssuer,
		ValidAudience = jwtAudience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
	};
});

builder.Services.AddAuthorization();

// ✅ 3. DI + DB
builder.Services.AddDbContext<VidyasetuAI_DevContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HelperService>();

// ✅ 4. Swagger + Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

// ✅ 5. Middleware pipeline (order matters!)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowFrontend");     // ✅ MUST be before Auth
app.UseAuthentication();          // ✅ REQUIRED for JWT auth
app.UseAuthorization();

app.MapControllers();
app.Run();
