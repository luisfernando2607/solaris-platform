// ══════════════════════════════════════════════════════════════════════
//  Integration/AuthIntegrationTests.cs
//  ─────────────────────────────────────────────────────────────────────
//  Tests de integración — suite "auth" de test.config.js.
//  Prueban el flujo completo: login → JWT → uso → logout.
// ══════════════════════════════════════════════════════════════════════

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SolarisPlatform.Tests.Integration.Common;

namespace SolarisPlatform.Tests.Integration;

public class AuthIntegrationTests(SolarisWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // ── "login-bad": credenciales malas → 400 o 401 ─────────────────
    [Fact]
    public async Task Login_CredencialesIncorrectas_DebeRetornar400O401()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            email    = "malo@test.com",
            password = "Mala123!"
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized);
    }

    // ── "login-empty": body vacío → 400 ─────────────────────────────
    [Fact]
    public async Task Login_CamposVacios_DebeRetornar400()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            email    = "",
            password = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "login con campos vacíos debe ser rechazado por el validator");
    }

    // ── "login-ok": credenciales correctas → 200 con JWT ────────────
    [Fact]
    public async Task Login_CredencialesCorrectas_DebeRetornarJwt()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            email    = TestConstants.AdminEmail,
            password = TestConstants.AdminPassword
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(body);
    }

    // ── "me": perfil del usuario autenticado → 200 ───────────────────
    [Fact]
    public async Task Me_ConTokenValido_DebeRetornar200()
    {
        var response = await GetAuthAsync("/api/Auth/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── "cambiar-password": body vacío → 400 ─────────────────────────
    [Fact]
    public async Task CambiarPassword_BodyVacio_DebeRetornar400()
    {
        var response = await PostAuthAsync("/api/Auth/cambiar-password", new { });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "body vacío debe fallar en la validación del controller");
    }

    // ── "recuperar-password": email inexistente → 200 o 404 ──────────
    [Fact]
    public async Task RecuperarPassword_EmailInexistente_DebeRetornar200O404()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/recuperar-password", new
        {
            email = TestConstants.Emails.Invalid
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound);
    }

    // ── "resetear-password": token falso → 400 o 404 ─────────────────
    [Fact]
    public async Task ResetearPassword_TokenFalso_DebeRetornar400O404()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/resetear-password", new
        {
            token           = "fake-token-runner",
            nuevaPassword   = "Solaris789!"
        });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound);
    }

    // ── "verificar-email": token falso → 400 o 404 ───────────────────
    [Fact]
    public async Task VerificarEmail_TokenFalso_DebeRetornar400O404()
    {
        var response = await Client.GetAsync("/api/Auth/verificar-email/fake-token-runner");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound);
    }

    // ── "logout": usuario autenticado → 200 ──────────────────────────
    [Fact]
    public async Task Logout_UsuarioAutenticado_DebeRetornar200()
    {
        var response = await PostAuthAsync("/api/Auth/logout", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
