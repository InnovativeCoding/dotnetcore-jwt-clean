using Blog.Infrastructure;
using Blog.Infrastructure.Authentication;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>();

var key = Encoding.UTF8.GetBytes(jwtOptions!.Key);


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
