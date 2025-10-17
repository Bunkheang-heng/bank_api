using System.Security.Claims;
using bank_api.Auth;
using bank_api.Contracts.Auth;
using bank_api.Data;
using bank_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace bank_api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth");

            group.MapPost("/register", RegisterAsync);
            group.MapPost("/login", LoginAsync);
            app.MapGet("/me", Me).RequireAuthorization();
        }

        private static async Task<Results<Created<object>, ValidationProblem, Conflict>> RegisterAsync(
            RegisterRequest req,
            PasswordHasher hasher,
            ApplicationDbContext db)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["username"] = new[] { "Username is required" },
                    ["password"] = new[] { "Password is required" }
                });
            }

            var exists = await db.Users.AnyAsync(u => u.Username == req.Username);
            if (exists)
            {
                return TypedResults.Conflict();
            }

            var passwordHash = hasher.HashPassword(req.Password);
            db.Users.Add(new User { Username = req.Username, PasswordHash = passwordHash });
            await db.SaveChangesAsync();

            return TypedResults.Created($"/users/{req.Username}", new { username = req.Username });
        }

        private static async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> LoginAsync(
            LoginRequest req,
            PasswordHasher hasher,
            ApplicationDbContext db,
            JwtTokenService tokens)
        {
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user is null)
            {
                await Task.Delay(Random.Shared.Next(50, 150));
                return TypedResults.Unauthorized();
            }

            if (!hasher.VerifyPassword(req.Password, user.PasswordHash))
            {
                return TypedResults.Unauthorized();
            }

            var (token, exp) = tokens.CreateToken(user.Username);
            return TypedResults.Ok(new AuthResponse
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresAtUtc = exp
            });
        }

        private static Results<Ok<object>, UnauthorizedHttpResult> Me(ClaimsPrincipal user)
        {
            if (user.Identity?.IsAuthenticated != true)
            {
                return TypedResults.Unauthorized();
            }
            var name = user.Identity?.Name ?? string.Empty;
            return TypedResults.Ok(new { username = name });
        }
    }
}


