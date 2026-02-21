using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DuelMastersApi.Data.Models;

namespace DuelMastersApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) { _config = config; }

        public string CreateToken(Player player)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
            var issuer = jwt["Issuer"] ?? "DuelMasters";
            var audience = jwt["Audience"] ?? "DuelMastersClients";
            var expire = int.TryParse(jwt["ExpireMinutes"], out var m) ? m : 60;

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, player.Username)
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expire),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
