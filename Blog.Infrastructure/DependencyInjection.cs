using Blog.Application.IServices;
using Blog.Infrastructure.Authentication;
using Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Blog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(
                configuration.GetSection("Jwt"));
            //inject db context
            services.AddDbContext<BlogDbContext>(options =>
           options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection")));

            // other services
            services.AddScoped<IApplicationDbContext>(
                provider => provider.GetRequiredService<BlogDbContext>());

            services.AddScoped<ITokenService, TokenService>();
            services.AddJwtAuthentication(configuration);
            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration
                .GetSection("Jwt")
                .Get<JwtOptions>()!;

            var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication Failed:");
                        Console.WriteLine(context.Exception.Message);
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        Console.WriteLine("OnChallenge error:");
                        Console.WriteLine(context.Error);
                        Console.WriteLine(context.ErrorDescription);
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
