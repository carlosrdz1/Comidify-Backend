using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Comidify.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace Comidify.API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(Usuario usuario)
    {
        // DEBUG
        Console.WriteLine("==============================================");
        Console.WriteLine("TOKEN SERVICE:");
        Console.WriteLine($"SigningKey: {_config["JWT:SigningKey"]}");
        Console.WriteLine($"Issuer: {_config["JWT:Issuer"]}");
        Console.WriteLine($"Audience: {_config["JWT:Audience"]}");
        Console.WriteLine("==============================================");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["JWT:SigningKey"] ?? throw new Exception("JWT:SigningKey not found")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}