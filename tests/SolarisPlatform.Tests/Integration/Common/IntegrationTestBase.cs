// ══════════════════════════════════════════════════════════════════════
//  Integration/Common/IntegrationTestBase.cs
//  ─────────────────────────────────────────────────────────────────────
//  Clase base que todos los integration tests heredan.
//  Provee: cliente HTTP, helper para login, asserts de status comunes.
// ══════════════════════════════════════════════════════════════════════

using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace SolarisPlatform.Tests.Integration.Common;

/// <summary>
/// Base para todos los integration tests.
/// Hereda de IClassFixture para compartir la factory entre tests del mismo archivo.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<SolarisWebApplicationFactory>
{
    protected readonly HttpClient Client;
    private string? _cachedToken;

    protected IntegrationTestBase(SolarisWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    // ── Obtener JWT (equivalente al login-ok de test.config.js) ─────
    protected async Task<string> GetTokenAsync(
        string email    = TestConstants.AdminEmail,
        string password = TestConstants.AdminPassword)
    {
        if (_cachedToken is not null) return _cachedToken;

        var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            email,
            password
        });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
            "el login de admin debe funcionar para poder correr los demás tests");

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _cachedToken = body?.Data?.Token ?? body?.Token
            ?? throw new InvalidOperationException("El login no retornó token JWT");

        return _cachedToken;
    }

    // ── Agrega el header Authorization al cliente ─────────────────
    protected async Task AuthenticateAsync()
    {
        var token = await GetTokenAsync();
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // ── Helper: POST con auth ─────────────────────────────────────
    protected async Task<HttpResponseMessage> PostAuthAsync<T>(string url, T body)
    {
        await AuthenticateAsync();
        return await Client.PostAsJsonAsync(url, body);
    }

    // ── Helper: GET con auth ──────────────────────────────────────
    protected async Task<HttpResponseMessage> GetAuthAsync(string url)
    {
        await AuthenticateAsync();
        return await Client.GetAsync(url);
    }

    // ── Helper: DELETE con auth ───────────────────────────────────
    protected async Task<HttpResponseMessage> DeleteAuthAsync(string url)
    {
        await AuthenticateAsync();
        return await Client.DeleteAsync(url);
    }

    // ── DTOs internos para deserializar el login ──────────────────
    private record LoginResponse(LoginData? Data, string? Token);
    private record LoginData(string Token, string RefreshToken);
}
