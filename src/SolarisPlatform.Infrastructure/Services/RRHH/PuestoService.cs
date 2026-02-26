using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class PuestoService : IPuestoService
{
    private readonly SolarisDbContext _db;
    private readonly IMapper _mapper;

    public PuestoService(SolarisDbContext db, IMapper mapper)
    { _db = db; _mapper = mapper; }

    public async Task<ApiResponse<IEnumerable<PuestoDto>>> ObtenerAsync(long empresaId, long? departamentoId = null)
    {
        var query = _db.Set<Puesto>().Where(p => p.EmpresaId == empresaId).Include(p => p.Departamento).AsQueryable();
        if (departamentoId.HasValue) query = query.Where(p => p.DepartamentoId == departamentoId.Value);
        var lista = await query.OrderBy(p => p.Departamento!.Nombre).ThenBy(p => p.Nombre).ToListAsync();
        return ApiResponse<IEnumerable<PuestoDto>>.Ok(_mapper.Map<IEnumerable<PuestoDto>>(lista));
    }

    public async Task<ApiResponse<PuestoDto>> ObtenerPorIdAsync(long id)
    {
        var p = await _db.Set<Puesto>().Include(p => p.Departamento).FirstOrDefaultAsync(p => p.Id == id);
        if (p == null) return ApiResponse<PuestoDto>.Fail("Puesto no encontrado");
        return ApiResponse<PuestoDto>.Ok(_mapper.Map<PuestoDto>(p));
    }

    public async Task<ApiResponse<PuestoDto>> CrearAsync(long empresaId, CrearPuestoRequest request)
    {
        if (await _db.Set<Puesto>().AnyAsync(p => p.EmpresaId == empresaId && p.Codigo == request.Codigo))
            return ApiResponse<PuestoDto>.Fail($"Ya existe un puesto con el código '{request.Codigo}'");
        if (!await _db.Set<Departamento>().AnyAsync(d => d.Id == request.DepartamentoId && d.EmpresaId == empresaId))
            return ApiResponse<PuestoDto>.Fail("El departamento especificado no existe");
        var entidad = new Puesto
        {
            EmpresaId = empresaId, DepartamentoId = request.DepartamentoId,
            Codigo = request.Codigo, Nombre = request.Nombre, Descripcion = request.Descripcion,
            NivelJerarquico = request.NivelJerarquico, BandaSalarialMin = request.BandaSalarialMin,
            BandaSalarialMax = request.BandaSalarialMax, MonedaId = request.MonedaId,
            RequiereTitulo = request.RequiereTitulo
        };
        _db.Set<Puesto>().Add(entidad);
        await _db.SaveChangesAsync();
        return await ObtenerPorIdAsync(entidad.Id);
    }

    public async Task<ApiResponse<PuestoDto>> ActualizarAsync(long id, ActualizarPuestoRequest request)
    {
        var entidad = await _db.Set<Puesto>().FindAsync(id);
        if (entidad == null) return ApiResponse<PuestoDto>.Fail("Puesto no encontrado");
        entidad.DepartamentoId = request.DepartamentoId; entidad.Nombre = request.Nombre;
        entidad.Descripcion = request.Descripcion; entidad.NivelJerarquico = request.NivelJerarquico;
        entidad.BandaSalarialMin = request.BandaSalarialMin; entidad.BandaSalarialMax = request.BandaSalarialMax;
        entidad.RequiereTitulo = request.RequiereTitulo;
        await _db.SaveChangesAsync();
        return await ObtenerPorIdAsync(id);
    }

    public async Task<ApiResponse> EliminarAsync(long id)
    {
        var entidad = await _db.Set<Puesto>().FindAsync(id);
        if (entidad == null) return ApiResponse.Fail("Puesto no encontrado");
        if (await _db.Set<Empleado>().AnyAsync(e => e.PuestoId == id && e.Estado != 4))
            return ApiResponse.Fail("No se puede eliminar un puesto con empleados activos asignados");
        _db.Set<Puesto>().Remove(entidad);
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Puesto eliminado correctamente");
    }
}
