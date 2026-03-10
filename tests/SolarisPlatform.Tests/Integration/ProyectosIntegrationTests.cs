// ══════════════════════════════════════════════════════════════════════
//  Integration/ProyectosIntegrationTests.cs
//  ─────────────────────────────────────────────────────────────────────
//  Tests de integración — suite "proyectos-core" de test.config.js.
//  Prueba el flujo encadenado completo: crear → leer → editar → borrar.
//  Equivalente exacto al patrón captureId/useDynamic del runner JS.
// ══════════════════════════════════════════════════════════════════════

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SolarisPlatform.Tests.Integration.Common;

namespace SolarisPlatform.Tests.Integration;

public class ProyectosIntegrationTests(SolarisWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // ════════════════════════════════════════════════════════════════
    //  FLUJO CRUD ENCADENADO
    //  (Equivalente al patrón captureId → useDynamic del runner JS)
    // ════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Proyectos_FlujoCrudCompleto_DebeCompletarseSinErrores()
    {
        await AuthenticateAsync();

        // ── PASO 1: Listar proyectos (list-proyectos) ────────────────
        var listResponse = await Client.GetAsync("/api/proy/proyectos");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // ── PASO 2: Crear proyecto (create-proyecto) ─────────────────
        // Equivalente exacto al bodyFn 'create-proyecto'
        var createResponse = await Client.PostAsJsonAsync("/api/proy/proyectos", new
        {
            codigo           = $"PROY{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
            nombre           = "Proyecto Test xUnit",
            descripcion      = "Proyecto de prueba desde xUnit",
            empresaId        = TestConstants.Seed.EmpresaId,
            tipoProyecto     = 1,
            prioridad        = 2,
            fechaInicioPlan  = "2025-01-01",
            fechaFinPlan     = "2025-12-31",
            presupuestoTotal = 100_000
        });

        createResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Created);

        // ── PASO 3: Capturar el ID generado (equivalente a captureId) ─
        var created = await createResponse.Content.ReadFromJsonAsync<ProyectoResponse>();
        var proyectoId = created?.Data?.Id ?? created?.Id;
        proyectoId.Should().HaveValue("el servidor debe retornar el ID del proyecto creado");

        // ── PASO 4: Leer el proyecto creado (get-proyecto-new) ────────
        var getResponse = await Client.GetAsync($"/api/proy/proyectos/{proyectoId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // ── PASO 5: Actualizar el proyecto (update-proyecto) ──────────
        var updateResponse = await Client.PutAsJsonAsync($"/api/proy/proyectos/{proyectoId}", new
        {
            nombre           = "Proyecto Editado xUnit",
            descripcion      = "Editado por xUnit",
            empresaId        = TestConstants.Seed.EmpresaId,
            tipoProyecto     = 1,
            prioridad        = 2,
            fechaInicioPlan  = "2025-01-01",
            fechaFinPlan     = "2025-12-31",
            presupuestoTotal = 120_000
        });

        updateResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent);

        // ── PASO 6: Dashboard del proyecto (proyecto-dashboard) ───────
        var dashResponse = await Client.GetAsync($"/api/proy/proyectos/{proyectoId}/dashboard");
        dashResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // ── PASO 7: Cambiar estado (patch-proyecto-estado) ────────────
        var estadoResponse = await Client.PatchAsJsonAsync(
            $"/api/proy/proyectos/{proyectoId}/estado",
            new { estado = 2 });
        estadoResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, HttpStatusCode.NoContent);

        // ── PASO 8: Cambiar avance (patch-proyecto-avance) ────────────
        var avanceResponse = await Client.PatchAsJsonAsync(
            $"/api/proy/proyectos/{proyectoId}/avance",
            new { avancePorcentaje = 10 });
        avanceResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, HttpStatusCode.NoContent);

        // ── PASO 9: Eliminar el proyecto (delete-proyecto) ────────────
        var deleteResponse = await Client.DeleteAsync($"/api/proy/proyectos/{proyectoId}");
        deleteResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent);
    }

    // ════════════════════════════════════════════════════════════════
    //  CASOS NEGATIVOS
    // ════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CrearProyecto_SinCamposRequeridos_DebeRetornar400()
    {
        // "post-proyecto-vacio": body:{} → 400
        var response = await PostAuthAsync("/api/proy/proyectos", new { });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProyectoSeed_DebeRetornar200O404()
    {
        // "get-proyecto-seed": proyecto semilla puede o no existir
        var response = await GetAuthAsync($"/api/proy/proyectos/{TestConstants.Seed.ProyectoId}");
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound);
    }

    // ── DTOs internos para deserializar respuestas ─────────────────
    private record ProyectoResponse(ProyectoData? Data, int? Id);
    private record ProyectoData(int Id, string Nombre);
}
