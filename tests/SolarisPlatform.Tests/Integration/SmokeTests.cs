// ══════════════════════════════════════════════════════════════════════
//  Integration/SmokeTests.cs
//  ─────────────────────────────────────────────────────────────────────
//  Tests de integración — suite "smoke" de test.config.js.
//  Verifican que la API arranca y responde en los endpoints básicos.
//  No necesitan servidor corriendo — usan WebApplicationFactory.
// ══════════════════════════════════════════════════════════════════════

using System.Net;
using FluentAssertions;
using SolarisPlatform.Tests.Integration.Common;

namespace SolarisPlatform.Tests.Integration;

public class SmokeTests(SolarisWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // ── Equivalente al test "health" de test.config.js ───────────────
    [Fact]
    public async Task Health_General_DebeRetornar200()
    {
        var response = await Client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_Api_DebeRetornar200()
    {
        var response = await Client.GetAsync("/health/api");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_Data_DebeRetornar200()
    {
        var response = await Client.GetAsync("/health/data");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Equivalente al test "unauth": endpoint protegido sin JWT → 401
    [Fact]
    public async Task Usuarios_SinToken_DebeRetornar401()
    {
        // Sin llamar AuthenticateAsync() → sin header Authorization
        var response = await Client.GetAsync("/api/Usuarios");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "los endpoints protegidos deben rechazar requests sin JWT");
    }

    // ── Equivalente al test "invalid-token": JWT basura → 401 ─────────
    [Fact]
    public async Task Usuarios_TokenInvalido_DebeRetornar401()
    {
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", "eyJhbGciOiJIUzI1NiJ9.FAKE.INVALID");

        var response = await Client.GetAsync("/api/Usuarios");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            "un JWT inválido debe ser rechazado con 401");
    }
}
