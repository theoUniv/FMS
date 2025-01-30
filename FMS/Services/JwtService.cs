using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Service responsable de la génération de tokens JWT.
/// </summary>
public class JwtService
{
    private readonly string _secretKey;
    private readonly int _expiryDurationInMinutes;

    /// <summary>
    /// Constructeur par défaut du service JWT. Initialise la clé secrète et la durée d'expiration du token.
    /// </summary>
    public JwtService()
    {
        _secretKey = "UneCleSecretePourLeJWTQuiEstLongueAssezPourHS256";
        _expiryDurationInMinutes = 30;
    }

    /// <summary>
    /// Génère un token JWT pour un utilisateur donné.
    /// </summary>
    /// <param name="username">Nom d'utilisateur pour lequel le token sera généré.</param>
    /// <returns>Token JWT sous forme de chaîne de caractères.</returns>
    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "FMS",
            audience: "FMS",
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expiryDurationInMinutes),
            signingCredentials: creds
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
