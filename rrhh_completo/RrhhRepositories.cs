using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Domain.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Persistence.Repositories.RRHH;

// ─────────────────────────────────────────────────────────────────────
// DEPARTAMENTO
// ─────────────────────────────────────────────────────────────────────

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly SolarisDbContext _db;
    public DepartamentoRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<Departamento>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true)
        => await _db.Departamentos
            .Where(d => d.EmpresaId == empresaId)
            .Include(d => d.DepartamentoPadre)
            .Include(d => d.SubDepartamentos)
            .Include(d => d.Responsable)
            .OrderBy(d => d.Nivel).ThenBy(d => d.Nombre)
            .ToListAsync();

    public async Task<Departamento?> ObtenerPorIdAsync(long id)
        => await _db.Departamentos
            .Include(d => d.DepartamentoPadre)
            .Include(d => d.Responsable)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Departamento?> ObtenerConHijosAsync(long id)
        => await _db.Departamentos
            .Include(d => d.DepartamentoPadre)
            .Include(d => d.SubDepartamentos)
            .Include(d => d.Responsable)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
        => await _db.Departamentos.AnyAsync(d =>
            d.EmpresaId == empresaId && d.Codigo == codigo &&
            (excludeId == null || d.Id != excludeId));

    public async Task<Departamento> CrearAsync(Departamento departamento)
    {
        _db.Departamentos.Add(departamento);
        await _db.SaveChangesAsync();
        return departamento;
    }

    public async Task<Departamento> ActualizarAsync(Departamento departamento)
    {
        _db.Departamentos.Update(departamento);
        await _db.SaveChangesAsync();
        return departamento;
    }

    public async Task EliminarAsync(long id)
    {
        var entidad = await _db.Departamentos.FindAsync(id);
        if (entidad != null)
        {
            _db.Departamentos.Remove(entidad);
            await _db.SaveChangesAsync();
        }
    }
}

// ─────────────────────────────────────────────────────────────────────
// PUESTO
// ─────────────────────────────────────────────────────────────────────

public class PuestoRepository : IPuestoRepository
{
    private readonly SolarisDbContext _db;
    public PuestoRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<Puesto>> ObtenerPorEmpresaAsync(long empresaId, long? departamentoId = null, bool soloActivos = true)
    {
        var q = _db.Puestos.Where(p => p.EmpresaId == empresaId).Include(p => p.Departamento).AsQueryable();
        if (departamentoId.HasValue) q = q.Where(p => p.DepartamentoId == departamentoId.Value);
        return await q.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Puesto?> ObtenerPorIdAsync(long id)
        => await _db.Puestos.Include(p => p.Departamento).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
        => await _db.Puestos.AnyAsync(p =>
            p.EmpresaId == empresaId && p.Codigo == codigo &&
            (excludeId == null || p.Id != excludeId));

    public async Task<Puesto> CrearAsync(Puesto puesto)
    {
        _db.Puestos.Add(puesto);
        await _db.SaveChangesAsync();
        return puesto;
    }

    public async Task<Puesto> ActualizarAsync(Puesto puesto)
    {
        _db.Puestos.Update(puesto);
        await _db.SaveChangesAsync();
        return puesto;
    }

    public async Task EliminarAsync(long id)
    {
        var entidad = await _db.Puestos.FindAsync(id);
        if (entidad != null) { _db.Puestos.Remove(entidad); await _db.SaveChangesAsync(); }
    }
}

// ─────────────────────────────────────────────────────────────────────
// EMPLEADO
// ─────────────────────────────────────────────────────────────────────

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly SolarisDbContext _db;
    public EmpleadoRepository(SolarisDbContext db) { _db = db; }

    public async Task<(IEnumerable<Empleado> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null)
    {
        var q = _db.Empleados
            .Where(e => e.EmpresaId == empresaId)
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var b = busqueda.ToLower();
            q = q.Where(e => e.NombreCompleto.ToLower().Contains(b) ||
                             e.Codigo.ToLower().Contains(b) ||
                             e.NumeroIdentificacion.ToLower().Contains(b));
        }
        if (departamentoId.HasValue) q = q.Where(e => e.DepartamentoId == departamentoId);
        if (puestoId.HasValue) q = q.Where(e => e.PuestoId == puestoId);
        if (estado.HasValue) q = q.Where(e => e.Estado == estado);

        var total = await q.CountAsync();
        var items = await q.OrderBy(e => e.NombreCompleto)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<Empleado?> ObtenerPorIdAsync(long id)
        => await _db.Empleados.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Empleado?> ObtenerFichaCompletaAsync(long id)
        => await _db.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .Include(e => e.JefeDirecto)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
        => await _db.Empleados.AnyAsync(e =>
            e.EmpresaId == empresaId && e.Codigo == codigo &&
            (excludeId == null || e.Id != excludeId));

    public async Task<bool> ExisteIdentificacionAsync(long empresaId, string tipo, string numero, long? excludeId = null)
        => await _db.Empleados.AnyAsync(e =>
            e.EmpresaId == empresaId &&
            e.TipoIdentificacion == tipo && e.NumeroIdentificacion == numero &&
            (excludeId == null || e.Id != excludeId));

    public async Task<IEnumerable<EmpleadoHistorial>> ObtenerHistorialAsync(long empleadoId)
        => await _db.EmpleadoHistorial
            .Where(h => h.EmpleadoId == empleadoId)
            .OrderByDescending(h => h.FechaEfectiva)
            .ToListAsync();

    public async Task<IEnumerable<EmpleadoDocumento>> ObtenerDocumentosAsync(long empleadoId)
        => await _db.EmpleadoDocumentos
            .Where(d => d.EmpleadoId == empleadoId)
            .OrderByDescending(d => d.Id)
            .ToListAsync();

    public async Task<Empleado> CrearAsync(Empleado empleado)
    {
        _db.Empleados.Add(empleado);
        await _db.SaveChangesAsync();
        // Generar código post-creación
        if (string.IsNullOrEmpty(empleado.Codigo))
        {
            empleado.Codigo = $"EMP-{empleado.Id:D5}";
            await _db.SaveChangesAsync();
        }
        return empleado;
    }

    public async Task<Empleado> ActualizarAsync(Empleado empleado)
    {
        _db.Empleados.Update(empleado);
        await _db.SaveChangesAsync();
        return empleado;
    }

    public async Task<EmpleadoHistorial> RegistrarHistorialAsync(EmpleadoHistorial historial)
    {
        _db.EmpleadoHistorial.Add(historial);
        await _db.SaveChangesAsync();
        return historial;
    }

    public async Task<EmpleadoDocumento> AgregarDocumentoAsync(EmpleadoDocumento documento)
    {
        _db.EmpleadoDocumentos.Add(documento);
        await _db.SaveChangesAsync();
        return documento;
    }

    public async Task EliminarDocumentoAsync(long documentoId)
    {
        var doc = await _db.EmpleadoDocumentos.FindAsync(documentoId);
        if (doc != null) { _db.EmpleadoDocumentos.Remove(doc); await _db.SaveChangesAsync(); }
    }
}

// ─────────────────────────────────────────────────────────────────────
// ASISTENCIA
// ─────────────────────────────────────────────────────────────────────

public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly SolarisDbContext _db;
    public AsistenciaRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<Asistencia>> ObtenerPorEmpleadoMesAsync(long empleadoId, int anno, int mes)
        => await _db.Asistencias
            .Where(a => a.EmpleadoId == empleadoId && a.Fecha.Year == anno && a.Fecha.Month == mes)
            .Include(a => a.Empleado)
            .OrderBy(a => a.Fecha).ToListAsync();

    public async Task<IEnumerable<Asistencia>> ObtenerPorEmpresaFechaAsync(long empresaId, DateOnly fecha)
        => await _db.Asistencias
            .Where(a => a.EmpresaId == empresaId && a.Fecha == fecha)
            .Include(a => a.Empleado)
            .ToListAsync();

    public async Task<Asistencia?> ObtenerPorEmpleadoFechaAsync(long empleadoId, DateOnly fecha)
        => await _db.Asistencias.FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Fecha == fecha);

    public async Task<Asistencia> CrearAsync(Asistencia asistencia)
    { _db.Asistencias.Add(asistencia); await _db.SaveChangesAsync(); return asistencia; }

    public async Task<Asistencia> ActualizarAsync(Asistencia asistencia)
    { _db.Asistencias.Update(asistencia); await _db.SaveChangesAsync(); return asistencia; }
}

public class SolicitudAusenciaRepository : ISolicitudAusenciaRepository
{
    private readonly SolarisDbContext _db;
    public SolicitudAusenciaRepository(SolarisDbContext db) { _db = db; }

    public async Task<(IEnumerable<SolicitudAusencia> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? empleadoId = null, short? estado = null)
    {
        var q = _db.SolicitudesAusencia.Where(s => s.EmpresaId == empresaId)
            .Include(s => s.Empleado).Include(s => s.Aprobador).AsQueryable();
        if (empleadoId.HasValue) q = q.Where(s => s.EmpleadoId == empleadoId);
        if (estado.HasValue) q = q.Where(s => s.Estado == estado);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(s => s.FechaInicio)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<SolicitudAusencia?> ObtenerPorIdAsync(long id)
        => await _db.SolicitudesAusencia
            .Include(s => s.Empleado).Include(s => s.Aprobador)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<SolicitudAusencia> CrearAsync(SolicitudAusencia solicitud)
    { _db.SolicitudesAusencia.Add(solicitud); await _db.SaveChangesAsync(); return solicitud; }

    public async Task<SolicitudAusencia> ActualizarAsync(SolicitudAusencia solicitud)
    { _db.SolicitudesAusencia.Update(solicitud); await _db.SaveChangesAsync(); return solicitud; }
}

public class SaldoVacacionesRepository : ISaldoVacacionesRepository
{
    private readonly SolarisDbContext _db;
    public SaldoVacacionesRepository(SolarisDbContext db) { _db = db; }

    public async Task<SaldoVacaciones?> ObtenerAsync(long empleadoId, int anno)
        => await _db.SaldosVacaciones.FirstOrDefaultAsync(s => s.EmpleadoId == empleadoId && s.Anno == anno);

    public async Task<IEnumerable<SaldoVacaciones>> ObtenerPorEmpleadoAsync(long empleadoId)
        => await _db.SaldosVacaciones.Where(s => s.EmpleadoId == empleadoId).OrderByDescending(s => s.Anno).ToListAsync();

    public async Task<SaldoVacaciones> CrearOActualizarAsync(SaldoVacaciones saldo)
    {
        var existente = await ObtenerAsync(saldo.EmpleadoId, saldo.Anno);
        if (existente == null) { _db.SaldosVacaciones.Add(saldo); }
        else { existente.DiasGanados = saldo.DiasGanados; existente.DiasTomados = saldo.DiasTomados; existente.DiasVencidos = saldo.DiasVencidos; }
        await _db.SaveChangesAsync();
        return existente ?? saldo;
    }
}

// ─────────────────────────────────────────────────────────────────────
// NÓMINA
// ─────────────────────────────────────────────────────────────────────

public class ConceptoNominaRepository : IConceptoNominaRepository
{
    private readonly SolarisDbContext _db;
    public ConceptoNominaRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<ConceptoNomina>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true)
        => await _db.ConceptosNomina.Where(c => c.EmpresaId == empresaId).OrderBy(c => c.OrdenCalculo).ToListAsync();

    public async Task<ConceptoNomina?> ObtenerPorIdAsync(long id)
        => await _db.ConceptosNomina.FindAsync(id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
        => await _db.ConceptosNomina.AnyAsync(c => c.EmpresaId == empresaId && c.Codigo == codigo && (excludeId == null || c.Id != excludeId));

    public async Task<ConceptoNomina> CrearAsync(ConceptoNomina concepto)
    { _db.ConceptosNomina.Add(concepto); await _db.SaveChangesAsync(); return concepto; }

    public async Task<ConceptoNomina> ActualizarAsync(ConceptoNomina concepto)
    { _db.ConceptosNomina.Update(concepto); await _db.SaveChangesAsync(); return concepto; }

    public async Task EliminarAsync(long id)
    { var e = await _db.ConceptosNomina.FindAsync(id); if (e != null) { _db.ConceptosNomina.Remove(e); await _db.SaveChangesAsync(); } }
}

public class PeriodoNominaRepository : IPeriodoNominaRepository
{
    private readonly SolarisDbContext _db;
    public PeriodoNominaRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<PeriodoNomina>> ObtenerPorEmpresaAnnoAsync(long empresaId, int anno)
        => await _db.PeriodosNomina.Where(p => p.EmpresaId == empresaId && p.Anno == anno)
            .OrderBy(p => p.NumeroPeriodo).ToListAsync();

    public async Task<PeriodoNomina?> ObtenerPorIdAsync(long id)
        => await _db.PeriodosNomina.FindAsync(id);

    public async Task<PeriodoNomina?> ObtenerActualAsync(long empresaId)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return await _db.PeriodosNomina.Where(p => p.EmpresaId == empresaId && p.Estado == 1 && p.FechaInicio <= hoy && p.FechaFin >= hoy)
            .FirstOrDefaultAsync();
    }

    public async Task<PeriodoNomina> CrearAsync(PeriodoNomina periodo)
    { _db.PeriodosNomina.Add(periodo); await _db.SaveChangesAsync(); return periodo; }

    public async Task<PeriodoNomina> ActualizarAsync(PeriodoNomina periodo)
    { _db.PeriodosNomina.Update(periodo); await _db.SaveChangesAsync(); return periodo; }
}

public class RolPagoRepository : IRolPagoRepository
{
    private readonly SolarisDbContext _db;
    public RolPagoRepository(SolarisDbContext db) { _db = db; }

    public async Task<(IEnumerable<RolPago> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? periodoId = null)
    {
        var q = _db.RolesPago.Where(r => r.EmpresaId == empresaId)
            .Include(r => r.Periodo).AsQueryable();
        if (periodoId.HasValue) q = q.Where(r => r.PeriodoId == periodoId);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.Id).Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<RolPago?> ObtenerPorIdAsync(long id)
        => await _db.RolesPago.Include(r => r.Periodo).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<RolPago?> ObtenerConDetalleAsync(long id)
        => await _db.RolesPago
            .Include(r => r.Periodo)
            .Include(r => r.Empleados).ThenInclude(e => e.Detalles).ThenInclude(d => d.Concepto)
            .Include(r => r.Empleados).ThenInclude(e => e.Empleado)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<string> GenerarNumeroAsync(long empresaId)
    {
        var ultimo = await _db.RolesPago.Where(r => r.EmpresaId == empresaId)
            .OrderByDescending(r => r.Id).Select(r => r.Numero).FirstOrDefaultAsync();
        int sig = 1;
        if (ultimo != null && int.TryParse(ultimo.Replace("ROL-", ""), out var n)) sig = n + 1;
        return $"ROL-{sig:D5}";
    }

    public async Task<RolPago> CrearAsync(RolPago rolPago)
    { _db.RolesPago.Add(rolPago); await _db.SaveChangesAsync(); return rolPago; }

    public async Task<RolPago> ActualizarAsync(RolPago rolPago)
    { _db.RolesPago.Update(rolPago); await _db.SaveChangesAsync(); return rolPago; }

    public async Task<RolPagoEmpleado> CrearEmpleadoAsync(RolPagoEmpleado empleado)
    { _db.RolesPagoEmpleado.Add(empleado); await _db.SaveChangesAsync(); return empleado; }

    public async Task<RolPagoEmpleado> ActualizarEmpleadoAsync(RolPagoEmpleado empleado)
    { _db.RolesPagoEmpleado.Update(empleado); await _db.SaveChangesAsync(); return empleado; }

    public async Task<IEnumerable<RolPagoDetalle>> ObtenerDetalleEmpleadoAsync(long rolPagoEmpleadoId)
        => await _db.RolesPagoDetalle.Where(d => d.RolPagoEmpleadoId == rolPagoEmpleadoId)
            .Include(d => d.Concepto).ToListAsync();

    public async Task GuardarDetallesAsync(IEnumerable<RolPagoDetalle> detalles)
    { _db.RolesPagoDetalle.AddRange(detalles); await _db.SaveChangesAsync(); }
}

public class ParametroNominaRepository : IParametroNominaRepository
{
    private readonly SolarisDbContext _db;
    public ParametroNominaRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<ParametroNomina>> ObtenerAsync(long empresaId, string paisCodigo, int anno)
        => await _db.ParametrosNomina.Where(p => p.EmpresaId == empresaId && p.PaisCodigo == paisCodigo && p.Anno == anno).ToListAsync();

    public async Task<ParametroNomina?> ObtenerAsync(long empresaId, string paisCodigo, int anno, string clave)
        => await _db.ParametrosNomina.FirstOrDefaultAsync(p => p.EmpresaId == empresaId && p.PaisCodigo == paisCodigo && p.Anno == anno && p.Clave == clave);

    public async Task<decimal> ObtenerValorDecimalAsync(long empresaId, string paisCodigo, int anno, string clave, decimal defaultValue = 0)
    {
        var p = await ObtenerAsync(empresaId, paisCodigo, anno, clave);
        return p != null && decimal.TryParse(p.Valor, out var v) ? v : defaultValue;
    }

    public async Task<ParametroNomina> CrearOActualizarAsync(ParametroNomina parametro)
    {
        var existente = await ObtenerAsync(parametro.EmpresaId, parametro.PaisCodigo, parametro.Anno, parametro.Clave);
        if (existente == null) { _db.ParametrosNomina.Add(parametro); await _db.SaveChangesAsync(); return parametro; }
        existente.Valor = parametro.Valor; existente.Descripcion = parametro.Descripcion;
        await _db.SaveChangesAsync(); return existente;
    }
}

// ─────────────────────────────────────────────────────────────────────
// PRÉSTAMOS
// ─────────────────────────────────────────────────────────────────────

public class PrestamoRepository : IPrestamoRepository
{
    private readonly SolarisDbContext _db;
    public PrestamoRepository(SolarisDbContext db) { _db = db; }

    public async Task<(IEnumerable<Prestamo> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? empleadoId = null, short? estado = null)
    {
        var q = _db.Prestamos.Where(p => p.EmpresaId == empresaId).Include(p => p.Empleado).AsQueryable();
        if (empleadoId.HasValue) q = q.Where(p => p.EmpleadoId == empleadoId);
        if (estado.HasValue) q = q.Where(p => p.Estado == estado);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.Id).Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<Prestamo?> ObtenerPorIdAsync(long id)
        => await _db.Prestamos.Include(p => p.Empleado).Include(p => p.Cuotas).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<PrestamoCuota>> ObtenerCuotasAsync(long prestamoId)
        => await _db.PrestamoCuotas.Where(c => c.PrestamoId == prestamoId).OrderBy(c => c.NumeroCuota).ToListAsync();

    public async Task<string> GenerarNumeroAsync(long empresaId)
    {
        var ultimo = await _db.Prestamos.Where(p => p.EmpresaId == empresaId)
            .OrderByDescending(p => p.Id).Select(p => p.Numero).FirstOrDefaultAsync();
        int sig = 1;
        if (ultimo != null && int.TryParse(ultimo.Replace("PRE-", ""), out var n)) sig = n + 1;
        return $"PRE-{sig:D5}";
    }

    public async Task<Prestamo> CrearAsync(Prestamo prestamo)
    { _db.Prestamos.Add(prestamo); await _db.SaveChangesAsync(); return prestamo; }

    public async Task<Prestamo> ActualizarAsync(Prestamo prestamo)
    { _db.Prestamos.Update(prestamo); await _db.SaveChangesAsync(); return prestamo; }

    public async Task<IEnumerable<PrestamoCuota>> ObtenerCuotasPendientesAsync(long empleadoId, DateOnly hasta)
        => await _db.PrestamoCuotas
            .Include(c => c.Prestamo)
            .Where(c => c.Prestamo!.EmpleadoId == empleadoId && c.Estado == 1 && c.FechaDescuento <= hasta)
            .OrderBy(c => c.FechaDescuento).ToListAsync();
}

// ─────────────────────────────────────────────────────────────────────
// EVALUACIÓN
// ─────────────────────────────────────────────────────────────────────

public class EvaluacionRepository : IEvaluacionRepository
{
    private readonly SolarisDbContext _db;
    public EvaluacionRepository(SolarisDbContext db) { _db = db; }

    public async Task<IEnumerable<EvaluacionProceso>> ObtenerProcesosAsync(long empresaId)
        => await _db.EvaluacionProcesos.Where(p => p.EmpresaId == empresaId)
            .Include(p => p.Plantilla).OrderByDescending(p => p.Anno).ToListAsync();

    public async Task<EvaluacionProceso?> ObtenerProcesoAsync(long id)
        => await _db.EvaluacionProcesos.Include(p => p.Plantilla).Include(p => p.Evaluaciones)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Evaluacion>> ObtenerPorEvaluadorAsync(long evaluadorId, long empresaId, short? estado = null)
    {
        var q = _db.Evaluaciones.Where(e => e.EvaluadorId == evaluadorId && e.EmpresaId == empresaId)
            .Include(e => e.Evaluado).Include(e => e.Proceso).AsQueryable();
        if (estado.HasValue) q = q.Where(e => e.Estado == estado);
        return await q.ToListAsync();
    }

    public async Task<Evaluacion?> ObtenerPorIdAsync(long id)
        => await _db.Evaluaciones.Include(e => e.Respuestas).ThenInclude(r => r.Criterio)
            .Include(e => e.Evaluado).Include(e => e.Evaluador).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<EvaluacionProceso> CrearProcesoAsync(EvaluacionProceso proceso)
    { _db.EvaluacionProcesos.Add(proceso); await _db.SaveChangesAsync(); return proceso; }

    public async Task<Evaluacion> CrearAsync(Evaluacion evaluacion)
    { _db.Evaluaciones.Add(evaluacion); await _db.SaveChangesAsync(); return evaluacion; }

    public async Task<Evaluacion> ActualizarAsync(Evaluacion evaluacion)
    { _db.Evaluaciones.Update(evaluacion); await _db.SaveChangesAsync(); return evaluacion; }

    public async Task GuardarRespuestasAsync(IEnumerable<EvaluacionRespuesta> respuestas)
    { _db.EvaluacionRespuestas.AddRange(respuestas); await _db.SaveChangesAsync(); }
}

// ─────────────────────────────────────────────────────────────────────
// CAPACITACIÓN
// ─────────────────────────────────────────────────────────────────────

public class CapacitacionRepository : ICapacitacionRepository
{
    private readonly SolarisDbContext _db;
    public CapacitacionRepository(SolarisDbContext db) { _db = db; }

    public async Task<(IEnumerable<Capacitacion> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, short? estado = null)
    {
        var q = _db.Capacitaciones.Where(c => c.EmpresaId == empresaId).AsQueryable();
        if (estado.HasValue) q = q.Where(c => c.Estado == estado);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(c => c.FechaInicio).Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<Capacitacion?> ObtenerPorIdAsync(long id)
        => await _db.Capacitaciones.FindAsync(id);

    public async Task<Capacitacion?> ObtenerConParticipantesAsync(long id)
        => await _db.Capacitaciones.Include(c => c.Participantes).ThenInclude(p => p.Empleado)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Capacitacion> CrearAsync(Capacitacion capacitacion)
    { _db.Capacitaciones.Add(capacitacion); await _db.SaveChangesAsync(); return capacitacion; }

    public async Task<Capacitacion> ActualizarAsync(Capacitacion capacitacion)
    { _db.Capacitaciones.Update(capacitacion); await _db.SaveChangesAsync(); return capacitacion; }

    public async Task InscribirEmpleadosAsync(IEnumerable<CapacitacionParticipante> participantes)
    { _db.CapacitacionParticipantes.AddRange(participantes); await _db.SaveChangesAsync(); }

    public async Task<CapacitacionParticipante> ActualizarParticipanteAsync(CapacitacionParticipante participante)
    { _db.CapacitacionParticipantes.Update(participante); await _db.SaveChangesAsync(); return participante; }
}
