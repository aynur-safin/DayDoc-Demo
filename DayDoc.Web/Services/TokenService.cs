using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.IdentityModel.Tokens;
using Parlot.Fluent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DayDoc.Web.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GetGwtToken(AppUser user)
        {
            var now = DateTime.UtcNow;

            _ = user.UserName ?? throw new ArgumentNullException(nameof(user.UserName));

            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("name", user.UserName), // ClaimsIdentity.DefaultNameClaimType
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            //var claims = new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //    new Claim(ClaimTypes.NameIdentifier, user.Id)
            //};

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Auth:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireMinutes = _config.GetValue<long>("Auth:LifetimeMinutes", 1440);

            //var claimsIdentity = new ClaimsIdentity(claims, "Token", JwtRegisteredClaimNames.Sub, null);

            var jwt = new JwtSecurityToken(
                issuer: _config["Auth:Issuer"],
                audience: _config["Auth:Audience"],
                claims: claims, // claimsIdentity.Claims,
                notBefore: now,
                expires: now.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
