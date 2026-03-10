using Microsoft.Extensions.Configuration;
using Moq;
using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Infrastructure.Services;

namespace SolarisPlatform.Tests.Unit.Infrastructure;

public class AuthServiceTests
{
    private static TokenService BuildTokenService()
    {
        var config = new Mock<IConfiguration>();
        // FIX: TokenService lee "Jwt:SecretKey", no "Jwt:Key"
        config.Setup(c => c["Jwt:SecretKey"]).Returns("SolarisPlatformSuperSecretKey2026!!Xtra");
        config.Setup(c => c["Jwt:Issuer"]).Returns("SolarisPlatform");
        config.Setup(c => c["Jwt:Audience"]).Returns("SolarisPlatformClients");
        config.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");
        return new TokenService(config.Object);
    }

    private static UsuarioAutenticadoDto BuildDto(long id = 1, string email = "test@solaris.com") => new()
    {
        Id = id, Email = email, NombreCompleto = "Test User",
        EmpresaId = 1, EmpresaNombre = "Empresa Test",
    };

    [Fact]
    public void GenerarToken_RetornaTokenNoVacio()
    {
        var token = BuildTokenService().GenerarToken(BuildDto());
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerarToken_TieneFormatoJwt()
    {
        Assert.Equal(3, BuildTokenService().GenerarToken(BuildDto()).Split('.').Length);
    }

    [Fact]
    public void GenerarToken_DosUsuarios_TokensDiferentes()
    {
        var svc = BuildTokenService();
        Assert.NotEqual(
            svc.GenerarToken(BuildDto(1, "test@solaris.com")),
            svc.GenerarToken(BuildDto(2, "otro@solaris.com")));
    }

    [Fact]
    public void GenerarRefreshToken_RetornaStringNoVacio()
    {
        Assert.NotEmpty(BuildTokenService().GenerarRefreshToken());
    }

    [Fact]
    public void GenerarRefreshToken_CadaLlamadaDistinto()
    {
        var svc = BuildTokenService();
        Assert.NotEqual(svc.GenerarRefreshToken(), svc.GenerarRefreshToken());
    }
}
