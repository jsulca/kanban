using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Kanban.WebApp.Commons;

internal static class TokenGenerator
{
    public static string GenerateTokenJwt(int id, string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JWT_SECRET_KEY));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claimsIdentity = new ClaimsIdentity([
            new Claim(ClaimTypes.PrimarySid, id.ToString()),
            new Claim(ClaimTypes.Name, username)
        ]);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
            AppSettings.JWT_ISSUER_TOKEN,
            AppSettings.JWT_AUDIENCE_TOKEN,
            claimsIdentity,
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1),
            signingCredentials: signingCredentials);

        return tokenHandler.WriteToken(jwtSecurityToken);
    }
}
