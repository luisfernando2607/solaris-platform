using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Domain.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Persistence.Repositories.RRHH;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly SolarisDbContext _ctx;
    public DepartamentoRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Departamento>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true)
    {
        return await _ctx.Set<Departamento>()
            .Include(d => d.SubDepartamentos)
            .Include(d => d.Responsable)
            .Where(d => d.EmpresaId == empresaId)
            .OrderBy(d => d.Nivel).ThenBy(d => d.Nombre)
            .ToListAsync();
    }

    public async Task<Departamento?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<Departamento>()
            .Include(d => d.Responsable)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Departamento?> ObtenerConHijosAsync(long id) =>
        await _ctx.Set<Departamento>()
            .Include(d => d.SubDepartamentos)
            .Include(d => d.Responsable)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
    {
        var q = _ctx.Set<Departamento>().Where(d => d.EmpresaId == empresaId && d.Codigo == codigo);
        if (excludeId.HasValue) q = q.Where(d => d.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<Departamento> CrearAsync(Departamento d)
    { _ctx.Set<Departamento>().Add(d); await _ctx.SaveChangesAsync(); return d; }

    public async Task<Departamento> ActualizarAsync(Departamento d)
    { _ctx.Set<Departamento>().Update(d); await _ctx.SaveChangesAsync(); return d; }

    public async Task EliminarAsync(long id)
    {
        var d = await _ctx.Set<Departamento>().FindAsync(id);
        if (d != null) { _ctx.Set<Departamento>().Remove(d); await _ctx.SaveChangesAsync(); }
    }
}

public class PuestoRepository : IPuestoRepository
{
    private readonly SolarisDbContext _ctx;
    public PuestoRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Puesto>> ObtenerPorEmpresaAsync(long empresaId, long? departamentoId = null, bool soloActivos = true)
    {
        var q = _ctx.Set<Puesto>()
            .Include(p => p.Departamento)
            .Where(p => p.EmpresaId == empresaId);
        if (departamentoId.HasValue) q = q.Where(p => p.DepartamentoId == departamentoId.Value);
        return await q.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Puesto?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<Puesto>().Include(p => p.Departamento).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
    {
        var q = _ctx.Set<Puesto>().Where(p => p.EmpresaId == empresaId && p.Codigo == codigo);
        if (excludeId.HasValue) q = q.Where(p => p.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<Puesto> CrearAsync(Puesto p)
    { _ctx.Set<Puesto>().Add(p); await _ctx.SaveChangesAsync(); return p; }

    public async Task<Puesto> ActualizarAsync(Puesto p)
    { _ctx.Set<Puesto>().Update(p); await _ctx.SaveChangesAsync(); return p; }

    public async Task EliminarAsync(long id)
    {
        var p = await _ctx.Set<Puesto>().FindAsync(id);
        if (p != null) { _ctx.Set<Puesto>().Remove(p); await _ctx.SaveChangesAsync(); }
    }
}

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly SolarisDbContext _ctx;
    public EmpleadoRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<Empleado> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null)
    {
        var q = _ctx.Set<Empleado>()
            .Include(e => e.Departamento).Include(e => e.Puesto)
            .Where(e => e.EmpresaId == empresaId);

        if (!string.IsNullOrEmpty(busqueda))
            q = q.Where(e => e.NombreCompleto.Contains(busqueda) ||
                             e.NumeroIdentificacion.Contains(busqueda) ||
                             e.Codigo.Contains(busqueda) ||
                             (e.EmailCorporativo != null && e.EmailCorporativo.Contains(busqueda)));

        if (departamentoId.HasValue) q = q.Where(e => e.DepartamentoId == departamentoId);
        if (puestoId.HasValue) q = q.Where(e => e.PuestoId == puestoId);
        if (estado.HasValue) q = q.Where(e => e.Estado == estado);

        var total = await q.CountAsync();
        var items = await q.OrderBy(e => e.PrimerApellido).ThenBy(e => e.PrimerNombre)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<Empleado?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<Empleado>()
            .Include(e => e.Departamento).Include(e => e.Puesto)
            .Include(e => e.JefeDirecto)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Empleado?> ObtenerFichaCompletaAsync(long id) =>
        await _ctx.Set<Empleado>()
            .Include(e => e.Departamento).Include(e => e.Puesto)
            .Include(e => e.JefeDirecto)
            .Include(e => e.Historial)
            .Include(e => e.Documentos)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
    {
        var q = _ctx.Set<Empleado>().Where(e => e.EmpresaId == empresaId && e.Codigo == codigo);
        if (excludeId.HasValue) q = q.Where(e => e.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<bool> ExisteIdentificacionAsync(long empresaId, string tipo, string numero, long? excludeId = null)
    {
        var q = _ctx.Set<Empleado>().Where(e => e.EmpresaId == empresaId &&
            e.TipoIdentificacion == tipo && e.NumeroIdentificacion == numero);
        if (excludeId.HasValue) q = q.Where(e => e.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<IEnumerable<EmpleadoHistorial>> ObtenerHistorialAsync(long empleadoId) =>
        await _ctx.Set<EmpleadoHistorial>()
            .Where(h => h.EmpleadoId == empleadoId)
            .OrderByDescending(h => h.FechaEfectiva).ToListAsync();

    public async Task<IEnumerable<EmpleadoDocumento>> ObtenerDocumentosAsync(long empleadoId) =>
        await _ctx.Set<EmpleadoDocumento>()
            .Where(d => d.EmpleadoId == empleadoId).ToListAsync();

    public async Task<Empleado> CrearAsync(Empleado e)
    { _ctx.Set<Empleado>().Add(e); await _ctx.SaveChangesAsync(); return e; }

    public async Task<Empleado> ActualizarAsync(Empleado e)
    { _ctx.Set<Empleado>().Update(e); await _ctx.SaveChangesAsync(); return e; }

    public async Task<EmpleadoHistorial> RegistrarHistorialAsync(EmpleadoHistorial h)
    { _ctx.Set<EmpleadoHistorial>().Add(h); await _ctx.SaveChangesAsync(); return h; }

    public async Task<EmpleadoDocumento> AgregarDocumentoAsync(EmpleadoDocumento d)
    { _ctx.Set<EmpleadoDocumento>().Add(d); await _ctx.SaveChangesAsync(); return d; }

    public async Task EliminarDocumentoAsync(long documentoId)
    {
        var d = await _ctx.Set<EmpleadoDocumento>().FindAsync(documentoId);
        if (d != null) { _ctx.Set<EmpleadoDocumento>().Remove(d); await _ctx.SaveChangesAsync(); }
    }

    public async Task<string> GenerarCodigoAsync(long empresaId)
    {
        var ultimo = await _ctx.Set<Empleado>()
            .Where(e => e.EmpresaId == empresaId)
            .OrderByDescending(e => e.Id)
            .Select(e => e.Codigo).FirstOrDefaultAsync();
        if (ultimo != null && int.TryParse(ultimo.Replace("EMP-", ""), out var num))
            return $"EMP-{(num + 1):D6}";
        return "EMP-000001";
    }
}

public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly SolarisDbContext _ctx;
    public AsistenciaRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Asistencia>> ObtenerPorEmpleadoMesAsync(long empleadoId, int anno, int mes) =>
        await _ctx.Set<Asistencia>()
            .Include(a => a.Empleado)
            .Where(a => a.EmpleadoId == empleadoId &&
                        a.Fecha.Year == anno && a.Fecha.Month == mes)
            .OrderBy(a => a.Fecha).ToListAsync();

    public async Task<IEnumerable<Asistencia>> ObtenerPorEmpresaFechaAsync(long empresaId, DateOnly fecha) =>
        await _ctx.Set<Asistencia>()
            .Include(a => a.Empleado)
            .Where(a => a.EmpresaId == empresaId && a.Fecha == fecha)
            .OrderBy(a => a.Empleado!.PrimerApellido).ToListAsync();

    public async Task<Asistencia?> ObtenerPorEmpleadoFechaAsync(long empleadoId, DateOnly fecha) =>
        await _ctx.Set<Asistencia>()
            .FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Fecha == fecha);

    public async Task<Asistencia> CrearAsync(Asistencia a)
    { _ctx.Set<Asistencia>().Add(a); await _ctx.SaveChangesAsync(); return a; }

    public async Task<Asistencia> ActualizarAsync(Asistencia a)
    { _ctx.Set<Asistencia>().Update(a); await _ctx.SaveChangesAsync(); return a; }
}

public class SolicitudAusenciaRepository : ISolicitudAusenciaRepository
{
    private readonly SolarisDbContext _ctx;
    public SolicitudAusenciaRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<SolicitudAusencia> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? empleadoId = null, short? estado = null)
    {
        var q = _ctx.Set<SolicitudAusencia>()
            .Include(s => s.Empleado).Include(s => s.Aprobador)
            .Where(s => s.EmpresaId == empresaId);
        if (empleadoId.HasValue) q = q.Where(s => s.EmpleadoId == empleadoId);
        if (estado.HasValue) q = q.Where(s => s.Estado == estado);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(s => s.FechaInicio)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<SolicitudAusencia?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<SolicitudAusencia>()
            .Include(s => s.Empleado).Include(s => s.Aprobador)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<SolicitudAusencia> CrearAsync(SolicitudAusencia s)
    { _ctx.Set<SolicitudAusencia>().Add(s); await _ctx.SaveChangesAsync(); return s; }

    public async Task<SolicitudAusencia> ActualizarAsync(SolicitudAusencia s)
    { _ctx.Set<SolicitudAusencia>().Update(s); await _ctx.SaveChangesAsync(); return s; }
}

public class ConceptoNominaRepository : IConceptoNominaRepository
{
    private readonly SolarisDbContext _ctx;
    public ConceptoNominaRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<ConceptoNomina>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true) =>
        await _ctx.Set<ConceptoNomina>()
            .Where(c => c.EmpresaId == empresaId)
            .OrderBy(c => c.OrdenCalculo).ThenBy(c => c.Nombre).ToListAsync();

    public async Task<ConceptoNomina?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<ConceptoNomina>().FindAsync(id);

    public async Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null)
    {
        var q = _ctx.Set<ConceptoNomina>().Where(c => c.EmpresaId == empresaId && c.Codigo == codigo);
        if (excludeId.HasValue) q = q.Where(c => c.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<ConceptoNomina> CrearAsync(ConceptoNomina c)
    { _ctx.Set<ConceptoNomina>().Add(c); await _ctx.SaveChangesAsync(); return c; }

    public async Task<ConceptoNomina> ActualizarAsync(ConceptoNomina c)
    { _ctx.Set<ConceptoNomina>().Update(c); await _ctx.SaveChangesAsync(); return c; }

    public async Task EliminarAsync(long id)
    {
        var c = await _ctx.Set<ConceptoNomina>().FindAsync(id);
        if (c != null) { _ctx.Set<ConceptoNomina>().Remove(c); await _ctx.SaveChangesAsync(); }
    }
}

public class RolPagoRepository : IRolPagoRepository
{
    private readonly SolarisDbContext _ctx;
    public RolPagoRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<RolPago> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? periodoId = null)
    {
        var q = _ctx.Set<RolPago>()
            .Include(r => r.Periodo)
            .Where(r => r.EmpresaId == empresaId);
        if (periodoId.HasValue) q = q.Where(r => r.PeriodoId == periodoId);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.Id)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return (items, total);
    }

    public async Task<RolPago?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<RolPago>()
            .Include(r => r.Periodo)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<RolPago?> ObtenerConDetalleAsync(long id) =>
        await _ctx.Set<RolPago>()
            .Include(r => r.Periodo)
            .Include(r => r.Empleados).ThenInclude(e => e.Empleado)
            .Include(r => r.Empleados).ThenInclude(e => e.Detalles).ThenInclude(d => d.Concepto)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<string> GenerarNumeroAsync(long empresaId)
    {
        var anno = DateTime.Today.Year;
        var count = await _ctx.Set<RolPago>()
            .CountAsync(r => r.EmpresaId == empresaId && r.FechaAprobacion == null
                          || r.EmpresaId == empresaId);
        return $"ROL-{anno}-{(count + 1):D4}";
    }

    public async Task<RolPago> CrearAsync(RolPago r)
    { _ctx.Set<RolPago>().Add(r); await _ctx.SaveChangesAsync(); return r; }

    public async Task<RolPago> ActualizarAsync(RolPago r)
    { _ctx.Set<RolPago>().Update(r); await _ctx.SaveChangesAsync(); return r; }

    public async Task<RolPagoEmpleado> CrearEmpleadoAsync(RolPagoEmpleado e)
    { _ctx.Set<RolPagoEmpleado>().Add(e); await _ctx.SaveChangesAsync(); return e; }

    public async Task<RolPagoEmpleado> ActualizarEmpleadoAsync(RolPagoEmpleado e)
    { _ctx.Set<RolPagoEmpleado>().Update(e); await _ctx.SaveChangesAsync(); return e; }

    public async Task<IEnumerable<RolPagoDetalle>> ObtenerDetalleEmpleadoAsync(long rolPagoEmpleadoId) =>
        await _ctx.Set<RolPagoDetalle>()
            .Include(d => d.Concepto)
            .Where(d => d.RolPagoEmpleadoId == rolPagoEmpleadoId)
            .OrderBy(d => d.Concepto!.OrdenCalculo).ToListAsync();

    public async Task GuardarDetallesAsync(IEnumerable<RolPagoDetalle> detalles)
    { _ctx.Set<RolPagoDetalle>().AddRange(detalles); await _ctx.SaveChangesAsync(); }
}

public class ParametroNominaRepository : IParametroNominaRepository
{
    private readonly SolarisDbContext _ctx;
    public ParametroNominaRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<ParametroNomina>> ObtenerAsync(long empresaId, string paisCodigo, int anno) =>
        await _ctx.Set<ParametroNomina>()
            .Where(p => p.EmpresaId == empresaId && p.PaisCodigo == paisCodigo && p.Anno == anno)
            .ToListAsync();

    public async Task<ParametroNomina?> ObtenerAsync(long empresaId, string paisCodigo, int anno, string clave) =>
        await _ctx.Set<ParametroNomina>()
            .FirstOrDefaultAsync(p => p.EmpresaId == empresaId && p.PaisCodigo == paisCodigo
                                   && p.Anno == anno && p.Clave == clave);

    public async Task<decimal> ObtenerValorDecimalAsync(long empresaId, string paisCodigo, int anno, string clave, decimal defaultValue = 0)
    {
        var p = await ObtenerAsync(empresaId, paisCodigo, anno, clave);
        if (p == null) return defaultValue;
        return decimal.TryParse(p.Valor, out var v) ? v : defaultValue;
    }

    public async Task<ParametroNomina> CrearOActualizarAsync(ParametroNomina parametro)
    {
        var existente = await ObtenerAsync(parametro.EmpresaId, parametro.PaisCodigo, parametro.Anno, parametro.Clave);
        if (existente == null)
            _ctx.Set<ParametroNomina>().Add(parametro);
        else
        {
            existente.Valor = parametro.Valor;
            existente.Descripcion = parametro.Descripcion;
        }
        await _ctx.SaveChangesAsync();
        return existente ?? parametro;
    }
}

public class PeriodoNominaRepository : IPeriodoNominaRepository
{
    private readonly SolarisDbContext _ctx;
    public PeriodoNominaRepository(SolarisDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<PeriodoNomina>> ObtenerPorEmpresaAnnoAsync(long empresaId, int anno) =>
        await _ctx.Set<PeriodoNomina>()
            .Where(p => p.EmpresaId == empresaId && p.Anno == anno)
            .OrderBy(p => p.NumeroPeriodo).ToListAsync();

    public async Task<PeriodoNomina?> ObtenerPorIdAsync(long id) =>
        await _ctx.Set<PeriodoNomina>().FindAsync(id);

    public async Task<PeriodoNomina?> ObtenerActualAsync(long empresaId)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return await _ctx.Set<PeriodoNomina>()
            .Where(p => p.EmpresaId == empresaId && p.FechaInicio <= hoy && p.FechaFin >= hoy)
            .FirstOrDefaultAsync();
    }

    public async Task<PeriodoNomina> CrearAsync(PeriodoNomina p)
    { _ctx.Set<PeriodoNomina>().Add(p); await _ctx.SaveChangesAsync(); return p; }

    public async Task<PeriodoNomina> ActualizarAsync(PeriodoNomina p)
    { _ctx.Set<PeriodoNomina>().Update(p); await _ctx.SaveChangesAsync(); return p; }
}
