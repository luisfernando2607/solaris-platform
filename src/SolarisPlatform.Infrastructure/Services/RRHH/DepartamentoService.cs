using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class DepartamentoService : IDepartamentoService
{
    private readonly SolarisDbContext _db;
    private readonly IMapper _mapper;

    public DepartamentoService(SolarisDbContext db, IMapper mapper)
    { _db = db; _mapper = mapper; }

    public async Task<ApiResponse<IEnumerable<DepartamentoDto>>> ObtenerArbolAsync(long empresaId)
    {
        var todos = await _db.Set<Departamento>()
            .Where(d => d.EmpresaId == empresaId)
            .Include(d => d.DepartamentoPadre)
            .Include(d => d.SubDepartamentos)
            .Include(d => d.Responsable)
            .OrderBy(d => d.Nivel).ThenBy(d => d.Nombre)
            .ToListAsync();
        var raices = todos.Where(d => d.DepartamentoPadreId == null)
            .Select(d => MapearConHijos(d, todos)).ToList();
        return ApiResponse<IEnumerable<DepartamentoDto>>.Ok(raices);
    }

    public async Task<ApiResponse<DepartamentoDto>> ObtenerPorIdAsync(long id)
    {
        var d = await _db.Set<Departamento>()
            .Include(d => d.DepartamentoPadre).Include(d => d.SubDepartamentos).Include(d => d.Responsable)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (d == null) return ApiResponse<DepartamentoDto>.Fail("Departamento no encontrado");
        return ApiResponse<DepartamentoDto>.Ok(MapearConHijos(d, d.SubDepartamentos.ToList()));
    }

    public async Task<ApiResponse<DepartamentoDto>> CrearAsync(long empresaId, CrearDepartamentoRequest request)
    {
        if (await _db.Set<Departamento>().AnyAsync(d => d.EmpresaId == empresaId && d.Codigo == request.Codigo))
            return ApiResponse<DepartamentoDto>.Fail($"Ya existe un departamento con el código '{request.Codigo}'");
        int nivel = 1;
        if (request.DepartamentoPadreId.HasValue)
        {
            var padre = await _db.Set<Departamento>().FindAsync(request.DepartamentoPadreId.Value);
            if (padre == null) return ApiResponse<DepartamentoDto>.Fail("Departamento padre no encontrado");
            nivel = padre.Nivel + 1;
        }
        var entidad = new Departamento
        {
            EmpresaId = empresaId, Codigo = request.Codigo, Nombre = request.Nombre,
            Descripcion = request.Descripcion, DepartamentoPadreId = request.DepartamentoPadreId,
            ResponsableId = request.ResponsableId, PresupuestoAnual = request.PresupuestoAnual, Nivel = nivel
        };
        _db.Set<Departamento>().Add(entidad);
        await _db.SaveChangesAsync();
        return await ObtenerPorIdAsync(entidad.Id);
    }

    public async Task<ApiResponse<DepartamentoDto>> ActualizarAsync(long id, ActualizarDepartamentoRequest request)
    {
        var entidad = await _db.Set<Departamento>().FindAsync(id);
        if (entidad == null) return ApiResponse<DepartamentoDto>.Fail("Departamento no encontrado");
        entidad.Nombre = request.Nombre; entidad.Descripcion = request.Descripcion;
        entidad.DepartamentoPadreId = request.DepartamentoPadreId;
        entidad.ResponsableId = request.ResponsableId; entidad.PresupuestoAnual = request.PresupuestoAnual;
        await _db.SaveChangesAsync();
        return await ObtenerPorIdAsync(id);
    }

    public async Task<ApiResponse> EliminarAsync(long id)
    {
        var entidad = await _db.Set<Departamento>().FindAsync(id);
        if (entidad == null) return ApiResponse.Fail("Departamento no encontrado");
        if (await _db.Set<Departamento>().AnyAsync(d => d.DepartamentoPadreId == id))
            return ApiResponse.Fail("No se puede eliminar un departamento con subdepartamentos");
        if (await _db.Set<Empleado>().AnyAsync(e => e.DepartamentoId == id))
            return ApiResponse.Fail("No se puede eliminar un departamento con empleados asignados");
        _db.Set<Departamento>().Remove(entidad);
        await _db.SaveChangesAsync();
        return ApiResponse.Ok(message: "Departamento eliminado correctamente");
    }

    private static DepartamentoDto MapearConHijos(Departamento d, IEnumerable<Departamento> hijos)
    {
        return new DepartamentoDto(
            d.Id, d.Codigo, d.Nombre, d.Descripcion,
            d.DepartamentoPadreId, d.DepartamentoPadre?.Nombre,
            d.ResponsableId, d.Responsable?.NombreCompleto,
            d.PresupuestoAnual, d.Nivel, true,
            hijos.Where(x => x.DepartamentoPadreId == d.Id)
                 .Select(x => MapearConHijos(x, x.SubDepartamentos)).ToList());
    }
}
