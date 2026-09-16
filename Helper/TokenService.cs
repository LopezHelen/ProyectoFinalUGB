using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public class TokenService(IOptions<JwtSettingsBase> options) : ITokenService
    {
        private readonly JwtSettingsBase settings = options.Value;

        public string GenerateToken(users user, IEnumerable<string> roleNames)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.email),
                new Claim(ClaimTypes.Email, user.email),
                new Claim("UserId", user.id.ToString())
            };

            foreach(string roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
