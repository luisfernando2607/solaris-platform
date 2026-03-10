// ══════════════════════════════════════════════════════════════════════
//  Integration/ErrorsAndSecurityTests.cs
//  ─────────────────────────────────────────────────────────────────────
//  Tests de integración — suite "errors" de test.config.js.
//  Verifican: 404s, validaciones de body vacío, tokens inválidos.
// ══════════════════════════════════════════════════════════════════════

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SolarisPlatform.Tests.Integration.Common;

namespace SolarisPlatform.Tests.Integration;

public class ErrorsAndSecurityTests(SolarisWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // ════════════════════════════════════════════════════════════════
    //  404s — recursos inexistentes
    // ════════════════════════════════════════════════════════════════

    // ── "404-usuario" ────────────────────────────────────────────────
    [Fact]
    public async Task GetUsuario_IdInexistente_DebeRetornar404()
    {
        var response = await GetAuthAsync("/api/Usuarios/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── "404-proyecto" ───────────────────────────────────────────────
    [Fact]
    public async Task GetProyecto_IdInexistente_DebeRetornar404()
    {
        var response = await GetAuthAsync("/api/proy/proyectos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── "404-empleado" ───────────────────────────────────────────────
    [Fact]
    public async Task GetEmpleado_IdInexistente_DebeRetornar404()
    {
        var response = await GetAuthAsync("/api/rrhh/empleados/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── "404-catalogo" ───────────────────────────────────────────────
    [Fact]
    public async Task GetPais_IdInexistente_DebeRetornar404()
    {
        var response = await GetAuthAsync("/api/catalogos/paises/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ════════════════════════════════════════════════════════════════
    //  VALIDACIONES — body vacío/inválido → 400
    // ════════════════════════════════════════════════════════════════

    // ── "post-proyecto-vacio" ────────────────────────────────────────
    [Fact]
    public async Task PostProyecto_BodyVacio_DebeRetornar400()
    {
        var response = await PostAuthAsync("/api/proy/proyectos", new { });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "el validator de proyecto debe rechazar un body vacío");
    }

    // ── "post-empleado-vacio" ────────────────────────────────────────
    [Fact]
    public async Task PostEmpleado_BodyVacio_DebeRetornar400()
    {
        var response = await PostAuthAsync("/api/rrhh/empleados", new { });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "el validator de empleado debe rechazar un body vacío");
    }

    // ── "login-empty" ────────────────────────────────────────────────
    [Fact]
    public async Task PostLogin_BodyVacio_DebeRetornar400()
    {
        var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            email    = "",
            password = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ════════════════════════════════════════════════════════════════
    //  SEGURIDAD — tokens inválidos
    // ════════════════════════════════════════════════════════════════

    // ── "invalid-token" ──────────────────────────────────────────────
    [Theory]
    [InlineData("eyJhbGciOiJIUzI1NiJ9.FAKE.INVALID")]  // JWT malformado
    [InlineData("Bearer fake-token")]                    // formato incorrecto
    [InlineData("just-garbage-text")]                    // basura
    public async Task GetEndpoint_TokenInvalido_DebeRetornar401(string fakeToken)
    {
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fakeToken);

        var response = await Client.GetAsync("/api/Usuarios");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Limpiar para no afectar otros tests
        Client.DefaultRequestHeaders.Authorization = null;
    }

    // ── "unauth": sin token en header ────────────────────────────────
    [Theory]
    [InlineData("/api/Usuarios")]
    [InlineData("/api/proy/proyectos")]
    [InlineData("/api/rrhh/empleados")]
    [InlineData("/api/catalogos/paises")]
    public async Task GetEndpointProtegido_SinToken_DebeRetornar401(string endpoint)
    {
        // Asegurarse de no tener token
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.GetAsync(endpoint);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
            $"el endpoint {endpoint} debe requerir autenticación");
    }
}
