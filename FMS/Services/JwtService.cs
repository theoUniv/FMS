using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secretKey = "UneCléSecrètePourLeJWTQuiEstLongueAssezPourHS256"; // Utilisez une clé plus longue (min 32 octets)
    private readonly int _expiryDurationInMinutes = 30;

    // Générer le token JWT
    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Assurez-vous que la clé a une longueur de 32 octets (256 bits)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.PadRight(32, ' '))); // Compléter à 32 octets si nécessaire
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "FMS",
            audience: "FMS",
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expiryDurationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
