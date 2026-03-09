using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.DTOs.Auth;

namespace SolarisPlatform.Infrastructure.Services;

/// <summary>
/// Servicio de generación y validación de tokens JWT
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey no configurado");
        _issuer = _configuration["Jwt:Issuer"] ?? "SolarisPlatform";
        _audience = _configuration["Jwt:Audience"] ?? "SolarisPlatformUsers";
        _expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    public string GenerarToken(UsuarioAutenticadoDto usuario)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("nombre", usuario.NombreCompleto),
            new("empresaId", usuario.EmpresaId.ToString()),
            new("empresaNombre", usuario.EmpresaNombre)
        };

        if (usuario.SucursalId.HasValue)
        {
            claims.Add(new Claim("sucursalId", usuario.SucursalId.Value.ToString()));
        }

        // Agregar roles
        foreach (var rol in usuario.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        // Agregar permisos
        foreach (var permiso in usuario.Permisos)
        {
            claims.Add(new Claim("permiso", permiso));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerarRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidarToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public long? ObtenerUsuarioIdDesdeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = false // No validar expiración para obtener el ID
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Servicio de hash de contraseñas usando BCrypt
/// </summary>
public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public string GenerarPasswordTemporal(int longitud = 12)
    {
        const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string minusculas = "abcdefghijklmnopqrstuvwxyz";
        const string numeros = "0123456789";
        const string especiales = "!@#$%^&*";
        
        var todos = mayusculas + minusculas + numeros + especiales;
        var random = new Random();
        var password = new StringBuilder();

        // Asegurar al menos uno de cada tipo
        password.Append(mayusculas[random.Next(mayusculas.Length)]);
        password.Append(minusculas[random.Next(minusculas.Length)]);
        password.Append(numeros[random.Next(numeros.Length)]);
        password.Append(especiales[random.Next(especiales.Length)]);

        // Completar el resto
        for (int i = 4; i < longitud; i++)
        {
            password.Append(todos[random.Next(todos.Length)]);
        }

        // Mezclar los caracteres
        return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
    }
}

/// <summary>
/// Servicio de fecha y hora
/// </summary>
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
