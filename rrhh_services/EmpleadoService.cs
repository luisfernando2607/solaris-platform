using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class EmpleadoService : IEmpleadoService
{
    private readonly SolarisDbContext _db;
    private readonly IMapper _mapper;

    public EmpleadoService(SolarisDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ── Consultas ─────────────────────────────────────────────────────

    public async Task<ApiResponse<PaginatedList<EmpleadoListaDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null)
    {
        var query = _db.Set<Empleado>()
            .Where(e => e.EmpresaId == empresaId && e.Activo)
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var q = busqueda.ToLower();
            query = query.Where(e =>
                e.NombreCompleto.ToLower().Contains(q) ||
                e.Codigo.ToLower().Contains(q) ||
                e.NumeroIdentificacion.ToLower().Contains(q) ||
                (e.EmailCorporativo != null && e.EmailCorporativo.ToLower().Contains(q)));
        }

        if (departamentoId.HasValue) query = query.Where(e => e.DepartamentoId == departamentoId);
        if (puestoId.HasValue)       query = query.Where(e => e.PuestoId == puestoId);
        if (estado.HasValue)         query = query.Where(e => e.Estado == estado);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.NombreCompleto)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        var dtos = _mapper.Map<List<EmpleadoListaDto>>(items);
        return ApiResponse<PaginatedList<EmpleadoListaDto>>.Ok(
            new PaginatedList<EmpleadoListaDto>(dtos, total, pagina, tamano));
    }

    public async Task<ApiResponse<EmpleadoFichaDto>> ObtenerFichaAsync(long id)
    {
        var e = await _db.Set<Empleado>()
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .Include(e => e.JefeDirecto)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null) return ApiResponse<EmpleadoFichaDto>.Fail("Empleado no encontrado");
        return ApiResponse<EmpleadoFichaDto>.Ok(_mapper.Map<EmpleadoFichaDto>(e));
    }

    // ── Creación ──────────────────────────────────────────────────────

    public async Task<ApiResponse<EmpleadoFichaDto>> CrearAsync(long empresaId, CrearEmpleadoRequest request)
    {
        if (await _db.Set<Empleado>().AnyAsync(e =>
            e.EmpresaId == empresaId && e.NumeroIdentificacion == request.NumeroIdentificacion))
            return ApiResponse<EmpleadoFichaDto>.Fail(
                $"Ya existe un empleado con la identificación '{request.NumeroIdentificacion}'");

        var codigo = await GenerarCodigoAsync(empresaId);

        var entidad = new Empleado
        {
            EmpresaId            = empresaId,
            Codigo               = codigo,
            TipoIdentificacion   = request.TipoIdentificacion,
            NumeroIdentificacion = request.NumeroIdentificacion,
            PrimerNombre         = request.PrimerNombre,
            SegundoNombre        = request.SegundoNombre,
            PrimerApellido       = request.PrimerApellido,
            SegundoApellido      = request.SegundoApellido,
            NombreCompleto       = $"{request.PrimerNombre} {request.SegundoNombre ?? ""} {request.PrimerApellido} {request.SegundoApellido ?? ""}".Trim().Replace("  ", " "),
            FechaNacimiento      = request.FechaNacimiento,
            Genero               = request.Genero,
            EstadoCivil          = request.EstadoCivil,
            NacionalidadPaisId   = request.NacionalidadPaisId,
            EmailPersonal        = request.EmailPersonal,
            EmailCorporativo     = request.EmailCorporativo,
            TelefonoCelular      = request.TelefonoCelular,
            TelefonoFijo         = request.TelefonoFijo,
            PaisId               = request.PaisId,
            EstadoProvinciaId    = request.EstadoProvinciaId,
            CiudadId             = request.CiudadId,
            Direccion            = request.Direccion,
            DepartamentoId       = request.DepartamentoId,
            PuestoId             = request.PuestoId,
            JefeDirectoId        = request.JefeDirectoId,
            FechaIngreso         = request.FechaIngreso,
            TipoContrato         = request.TipoContrato,
            ModalidadTrabajo     = request.ModalidadTrabajo,
            JornadaLaboral       = request.JornadaLaboral,
            HorasSemanales       = request.HorasSemanales,
            SalarioBase          = request.SalarioBase,
            NumeroSeguroSocial   = request.NumeroSeguroSocial,
            NumeroAfiliacion     = request.NumeroAfiliacion,
            BancoId              = request.BancoId,
            TipoCuentaBancaria   = request.TipoCuentaBancaria,
            NumeroCuentaBancaria = request.NumeroCuentaBancaria,
            FotoUrl              = request.FotoUrl,
            UsuarioId            = request.UsuarioId,
            Estado               = 1, // Activo
            Activo               = true,
            FechaCreacion        = DateTime.UtcNow
        };

        _db.Set<Empleado>().Add(entidad);
        await _db.SaveChangesAsync();
        return await ObtenerFichaAsync(entidad.Id);
    }

    // ── Actualización ─────────────────────────────────────────────────

    public async Task<ApiResponse<EmpleadoFichaDto>> ActualizarAsync(long id, ActualizarEmpleadoRequest request)
    {
        var entidad = await _db.Set<Empleado>().FindAsync(id);
        if (entidad == null) return ApiResponse<EmpleadoFichaDto>.Fail("Empleado no encontrado");

        entidad.PrimerNombre         = request.PrimerNombre;
        entidad.SegundoNombre        = request.SegundoNombre;
        entidad.PrimerApellido       = request.PrimerApellido;
        entidad.SegundoApellido      = request.SegundoApellido;
        entidad.NombreCompleto       = $"{request.PrimerNombre} {request.SegundoNombre ?? ""} {request.PrimerApellido} {request.SegundoApellido ?? ""}".Trim().Replace("  ", " ");
        entidad.FechaNacimiento      = request.FechaNacimiento;
        entidad.Genero               = request.Genero;
        entidad.EstadoCivil          = request.EstadoCivil;
        entidad.EmailPersonal        = request.EmailPersonal;
        entidad.EmailCorporativo     = request.EmailCorporativo;
        entidad.TelefonoCelular      = request.TelefonoCelular;
        entidad.TelefonoFijo         = request.TelefonoFijo;
        entidad.PaisId               = request.PaisId;
        entidad.EstadoProvinciaId    = request.EstadoProvinciaId;
        entidad.CiudadId             = request.CiudadId;
        entidad.Direccion            = request.Direccion;
        entidad.DepartamentoId       = request.DepartamentoId;
        entidad.PuestoId             = request.PuestoId;
        entidad.JefeDirectoId        = request.JefeDirectoId;
        entidad.TipoContrato         = request.TipoContrato;
        entidad.ModalidadTrabajo     = request.ModalidadTrabajo;
        entidad.HorasSemanales       = request.HorasSemanales;
        entidad.NumeroSeguroSocial   = request.NumeroSeguroSocial;
        entidad.NumeroAfiliacion     = request.NumeroAfiliacion;
        entidad.BancoId              = request.BancoId;
        entidad.TipoCuentaBancaria   = request.TipoCuentaBancaria;
        entidad.NumeroCuentaBancaria = request.NumeroCuentaBancaria;
        entidad.FotoUrl              = request.FotoUrl;
        entidad.FechaModificacion    = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await ObtenerFichaAsync(id);
    }

    // ── Acciones ──────────────────────────────────────────────────────

    public async Task<ApiResponse> EgresarAsync(long id, EgresarEmpleadoRequest request)
    {
        var entidad = await _db.Set<Empleado>().FindAsync(id);
        if (entidad == null) return ApiResponse.Fail("Empleado no encontrado");
        if (entidad.Estado == 4) return ApiResponse.Fail("El empleado ya está egresado");

        entidad.Estado            = 4; // Egresado
        entidad.FechaEgreso       = request.FechaEgreso;
        entidad.MotivoEgreso      = request.Motivo;
        entidad.FechaModificacion = DateTime.UtcNow;

        await RegistrarHistorialAsync(id, 3, "EGRESO", request.Motivo);
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Empleado egresado correctamente");
    }

    public async Task<ApiResponse> CambiarSalarioAsync(long id, CambiarSalarioRequest request)
    {
        var entidad = await _db.Set<Empleado>().FindAsync(id);
        if (entidad == null) return ApiResponse.Fail("Empleado no encontrado");

        var salarioAnterior = entidad.SalarioBase;
        entidad.SalarioBase       = request.NuevoSalario;
        entidad.FechaModificacion = DateTime.UtcNow;

        await RegistrarHistorialAsync(id, 2, "CAMBIO_SALARIO",
            $"Salario anterior: {salarioAnterior} → Nuevo: {request.NuevoSalario}. {request.Motivo}");
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Salario actualizado correctamente");
    }

    public async Task<ApiResponse> CambiarPuestoAsync(long id, CambiarPuestoRequest request)
    {
        var entidad = await _db.Set<Empleado>().FindAsync(id);
        if (entidad == null) return ApiResponse.Fail("Empleado no encontrado");

        var puestoAnteriorId = entidad.PuestoId;
        entidad.DepartamentoId    = request.NuevoDepartamentoId ?? entidad.DepartamentoId;
        entidad.PuestoId          = request.NuevoPuestoId;
        entidad.FechaModificacion = DateTime.UtcNow;

        await RegistrarHistorialAsync(id, 1, "CAMBIO_PUESTO",
            $"Puesto anterior: {puestoAnteriorId} → Nuevo: {request.NuevoPuestoId}. {request.Motivo}");
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Puesto actualizado correctamente");
    }

    // ── Historial ─────────────────────────────────────────────────────

    public async Task<ApiResponse<IEnumerable<EmpleadoHistorialDto>>> ObtenerHistorialAsync(long id)
    {
        var historial = await _db.Set<EmpleadoHistorial>()
            .Where(h => h.EmpleadoId == id)
            .OrderByDescending(h => h.FechaRegistro)
            .ToListAsync();

        return ApiResponse<IEnumerable<EmpleadoHistorialDto>>.Ok(
            _mapper.Map<IEnumerable<EmpleadoHistorialDto>>(historial));
    }

    // ── Documentos ────────────────────────────────────────────────────

    public async Task<ApiResponse<IEnumerable<EmpleadoDocumentoDto>>> ObtenerDocumentosAsync(long id)
    {
        var docs = await _db.Set<EmpleadoDocumento>()
            .Where(d => d.EmpleadoId == id)
            .OrderByDescending(d => d.FechaSubida)
            .ToListAsync();

        return ApiResponse<IEnumerable<EmpleadoDocumentoDto>>.Ok(
            _mapper.Map<IEnumerable<EmpleadoDocumentoDto>>(docs));
    }

    public async Task<ApiResponse<EmpleadoDocumentoDto>> AgregarDocumentoAsync(long id, AgregarDocumentoRequest request)
    {
        var empleadoExiste = await _db.Set<Empleado>().AnyAsync(e => e.Id == id);
        if (!empleadoExiste) return ApiResponse<EmpleadoDocumentoDto>.Fail("Empleado no encontrado");

        var doc = _mapper.Map<EmpleadoDocumento>(request);
        doc.EmpleadoId  = id;
        doc.FechaSubida = DateTime.UtcNow;

        _db.Set<EmpleadoDocumento>().Add(doc);
        await _db.SaveChangesAsync();

        return ApiResponse<EmpleadoDocumentoDto>.Ok(_mapper.Map<EmpleadoDocumentoDto>(doc));
    }

    public async Task<ApiResponse> EliminarDocumentoAsync(long documentoId)
    {
        var doc = await _db.Set<EmpleadoDocumento>().FindAsync(documentoId);
        if (doc == null) return ApiResponse.Fail("Documento no encontrado");

        _db.Set<EmpleadoDocumento>().Remove(doc);
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Documento eliminado correctamente");
    }

    // ── Helpers privados ──────────────────────────────────────────────

    private async Task<string> GenerarCodigoAsync(long empresaId)
    {
        var ultimo = await _db.Set<Empleado>()
            .Where(e => e.EmpresaId == empresaId)
            .OrderByDescending(e => e.Id)
            .Select(e => e.Codigo)
            .FirstOrDefaultAsync();

        int siguiente = 1;
        if (ultimo != null && int.TryParse(ultimo.Replace("EMP-", ""), out var num))
            siguiente = num + 1;

        return $"EMP-{siguiente:D5}";
    }

    private async Task RegistrarHistorialAsync(long empleadoId, short tipoCambio, string descripcion, string? detalle)
    {
        var historial = new EmpleadoHistorial
        {
            EmpleadoId    = empleadoId,
            TipoCambio    = tipoCambio,
            Descripcion   = descripcion,
            Detalle       = detalle,
            FechaRegistro = DateTime.UtcNow
        };
        _db.Set<EmpleadoHistorial>().Add(historial);
    }
}
