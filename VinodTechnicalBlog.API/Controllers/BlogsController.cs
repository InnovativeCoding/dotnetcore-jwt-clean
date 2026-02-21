using Blog.Application.IServices;
using Blog.Domain;
using Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VinodTechnicalBlog.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IApplicationDbContext _context;

        private readonly ILogger<BlogsController> _logger;

        public BlogsController(ILogger<BlogsController> logger, ITokenService tokenService, IApplicationDbContext context)
        {
            _tokenService = tokenService;
            _logger = logger;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // 1️⃣ Find user
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // 3️⃣ Generate access token
            var accessToken = _tokenService.GenerateAccessToken(user);

            // 4️⃣ Generate refresh token
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            // IMPORTANT:
            // If you hash refresh tokens inside service,
            // return RAW token separately (modify service accordingly)

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(CancellationToken.None);

            // 5️⃣ Return tokens
            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token // raw if not hashed
            });
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55)
            })
            .ToArray();
        }
    }
}
