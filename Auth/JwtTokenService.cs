using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace bank_api.Auth
{
    public class JwtTokenService
    {
        private readonly string issuer;
        private readonly string audience;
        private readonly string signingKey;
        private readonly int expiresMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
            audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
            signingKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
            if (!int.TryParse(configuration["Jwt:ExpiresMinutes"], out expiresMinutes))
            {
                expiresMinutes = 60;
            }
        }

        public (string token, DateTime expiresAtUtc) CreateToken(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);
        }
    }
}
