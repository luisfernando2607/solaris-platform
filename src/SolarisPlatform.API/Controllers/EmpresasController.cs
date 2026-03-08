using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.API.Controllers;

/// <summary>
/// Controller de empresas y sucursales
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly ISucursalRepository _sucursalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<EmpresasController> _logger;
    // ✅ FIX: Inyectar SolarisDbContext para manejar el tracking en Update/Delete de sucursales
    private readonly SolarisDbContext _context;

    public EmpresasController(
        IEmpresaRepository empresaRepository,
        ISucursalRepository sucursalRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<EmpresasController> logger,
        SolarisDbContext context)
    {
        _empresaRepository = empresaRepository;
        _sucursalRepository = sucursalRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
        _context = context;
    }

    #region Empresas

    /// <summary>
    /// Obtener todas las empresas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmpresaDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        // Solo super admin puede ver todas las empresas
        if (!_currentUserService.TieneRol("SUPER_ADMIN"))
        {
            var empresaId = _currentUserService.EmpresaId;
            if (!empresaId.HasValue)
            {
                return Forbid();
            }

            var empresa = await _empresaRepository.GetByIdAsync(empresaId.Value, cancellationToken);
            if (empresa == null)
            {
                return NotFound(ApiResponse<IEnumerable<EmpresaDto>>.Fail("Empresa no encontrada"));
            }

            return Ok(ApiResponse<IEnumerable<EmpresaDto>>.Ok(new[] { MapToDto(empresa) }));
        }

        var empresas = await _empresaRepository.GetAllAsync(cancellationToken);
        var dtos = empresas.Select(MapToDto);
        return Ok(ApiResponse<IEnumerable<EmpresaDto>>.Ok(dtos));
    }

    /// <summary>
    /// Obtener empresa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmpresaDto>>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            id != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var empresa = await _empresaRepository.GetWithSucursalesAsync(id, cancellationToken);

        if (empresa == null)
        {
            return NotFound(ApiResponse<EmpresaDto>.Fail("Empresa no encontrada"));
        }

        return Ok(ApiResponse<EmpresaDto>.Ok(MapToDto(empresa)));
    }

    /// <summary>
    /// Crear empresa (solo SUPER_ADMIN)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmpresaDto>>> Create(
        [FromBody] CrearEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.TieneRol("SUPER_ADMIN"))
        {
            return Forbid();
        }

        // Verificar código único
        if (await _empresaRepository.CodigoExisteAsync(request.Codigo, null, cancellationToken))
        {
            return BadRequest(ApiResponse<EmpresaDto>.Fail("Ya existe una empresa con ese código"));
        }

        // Verificar RUC único
        if (await _empresaRepository.RucExisteAsync(request.NumeroIdentificacion, null, cancellationToken))
        {
            return BadRequest(ApiResponse<EmpresaDto>.Fail("Ya existe una empresa con ese número de identificación"));
        }

        var empresa = new Empresa
        {
            Codigo = request.Codigo,
            TipoIdentificacion = request.TipoIdentificacion,
            NumeroIdentificacion = request.NumeroIdentificacion,
            RazonSocial = request.RazonSocial,
            NombreComercial = request.NombreComercial,
            PaisId = request.PaisId,
            DireccionFiscal = request.DireccionFiscal,
            Telefono = request.Telefono,
            Email = request.Email,
            PaginaWeb = request.PaginaWeb,
            Logo = request.Logo,
            MonedaPrincipalId = request.MonedaPrincipalId,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            UsuarioCreacion = _currentUserService.UsuarioId ?? 0
        };

        await _empresaRepository.AddAsync(empresa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Empresa {Codigo} creada", request.Codigo);
        return CreatedAtAction(
            nameof(GetById),
            new { id = empresa.Id },
            ApiResponse<EmpresaDto>.Ok(MapToDto(empresa), "Empresa creada exitosamente"));
    }

    /// <summary>
    /// Actualizar empresa
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<EmpresaDto>>> Update(
        long id,
        [FromBody] ActualizarEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        if (id != request.Id)
        {
            return BadRequest(ApiResponse<EmpresaDto>.Fail("El ID no coincide"));
        }

        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            id != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        if (empresa == null)
        {
            return NotFound(ApiResponse<EmpresaDto>.Fail("Empresa no encontrada"));
        }

        // Actualizar campos
        empresa.RazonSocial = request.RazonSocial;
        empresa.NombreComercial = request.NombreComercial;
        empresa.DireccionFiscal = request.DireccionFiscal;
        empresa.Telefono = request.Telefono;
        empresa.Email = request.Email;
        empresa.PaginaWeb = request.PaginaWeb;
        empresa.Logo = request.Logo;
        empresa.FechaModificacion = DateTime.UtcNow;
        empresa.UsuarioModificacion = _currentUserService.UsuarioId;

        await _empresaRepository.UpdateAsync(empresa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Empresa {Id} actualizada", id);
        return Ok(ApiResponse<EmpresaDto>.Ok(MapToDto(empresa), "Empresa actualizada exitosamente"));
    }

    #endregion

    #region Sucursales

    /// <summary>
    /// Obtener sucursales de una empresa
    /// </summary>
    [HttpGet("{empresaId}/sucursales")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SucursalDto>>>> GetSucursales(
        long empresaId,
        CancellationToken cancellationToken)
    {
        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            empresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var sucursales = await _sucursalRepository.GetByEmpresaAsync(empresaId, cancellationToken);
        var dtos = sucursales.Select(MapSucursalToDto);
        return Ok(ApiResponse<IEnumerable<SucursalDto>>.Ok(dtos));
    }

    /// <summary>
    /// Obtener sucursal por ID
    /// </summary>
    [HttpGet("{empresaId}/sucursales/{id}")]
    public async Task<ActionResult<ApiResponse<SucursalDto>>> GetSucursalById(
        long empresaId,
        long id,
        CancellationToken cancellationToken)
    {
        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            empresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var sucursal = await _sucursalRepository.GetByIdAsync(id, cancellationToken);
        if (sucursal == null || sucursal.EmpresaId != empresaId)
        {
            return NotFound(ApiResponse<SucursalDto>.Fail("Sucursal no encontrada"));
        }

        return Ok(ApiResponse<SucursalDto>.Ok(MapSucursalToDto(sucursal)));
    }

    /// <summary>
    /// Crear sucursal
    /// </summary>
    [HttpPost("{empresaId}/sucursales")]
    public async Task<ActionResult<ApiResponse<SucursalDto>>> CreateSucursal(
        long empresaId,
        [FromBody] CrearSucursalRequest request,
        CancellationToken cancellationToken)
    {
        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            empresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        // Verificar que existe la empresa
        var empresa = await _empresaRepository.GetByIdAsync(empresaId, cancellationToken);
        if (empresa == null)
        {
            return NotFound(ApiResponse<SucursalDto>.Fail("Empresa no encontrada"));
        }

        // Auto-generar Codigo si no viene en el request
        if (string.IsNullOrWhiteSpace(request.Codigo))
        {
            request.Codigo = $"SUC-{empresaId:D3}-{DateTime.UtcNow:yyMMddHHmm}";
        }

        // Verificar código único
        var sucursales = await _sucursalRepository.GetByEmpresaAsync(empresaId, cancellationToken);
        if (sucursales.Any(s => s.Codigo == request.Codigo && s.Activo))
        {
            return BadRequest(ApiResponse<SucursalDto>.Fail("Ya existe una sucursal con ese código en esta empresa"));
        }

        var sucursal = new Sucursal
        {
            EmpresaId = empresaId,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Direccion = request.Direccion,
            CiudadId = request.CiudadId,
            Telefono = request.Telefono,
            Email = request.Email,
            EsPrincipal = request.EsPrincipal,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            UsuarioCreacion = _currentUserService.UsuarioId ?? 0
        };

        // Si es principal, desmarcar las demás
        if (sucursal.EsPrincipal)
        {
            foreach (var s in sucursales.Where(s => s.EsPrincipal))
            {
                s.EsPrincipal = false;
                await _sucursalRepository.UpdateAsync(s, cancellationToken);
            }
        }

        await _sucursalRepository.AddAsync(sucursal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sucursal {Codigo} creada para empresa {EmpresaId}", request.Codigo, empresaId);
        return CreatedAtAction(
            nameof(GetSucursalById),
            new { empresaId, id = sucursal.Id },
            ApiResponse<SucursalDto>.Ok(MapSucursalToDto(sucursal), "Sucursal creada exitosamente"));
    }

    /// <summary>
    /// Actualizar sucursal
    /// </summary>
    [HttpPut("{empresaId}/sucursales/{id}")]
    public async Task<ActionResult<ApiResponse<SucursalDto>>> UpdateSucursal(
        long empresaId,
        long id,
        [FromBody] ActualizarSucursalRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        if (id != request.Id)
        {
            return BadRequest(ApiResponse<SucursalDto>.Fail("El ID no coincide"));
        }

        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            empresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        // ✅ FIX: Buscar con tracking directo desde _context en lugar del repository (que usa AsNoTracking)
        // Antes: _sucursalRepository.GetByIdAsync devolvía entidad sin tracking → SaveChanges fallaba con 500
        var sucursal = await _context.Sucursales
            .FirstOrDefaultAsync(s => s.Id == id && s.EmpresaId == empresaId, cancellationToken);

        if (sucursal == null)
        {
            return NotFound(ApiResponse<SucursalDto>.Fail("Sucursal no encontrada"));
        }

        // Si se marca como principal, desmarcar las demás usando _context para tracking consistente
        if (request.EsPrincipal && !sucursal.EsPrincipal)
        {
            var otrasPrincipales = await _context.Sucursales
                .Where(s => s.EmpresaId == empresaId && s.EsPrincipal && s.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var s in otrasPrincipales)
            {
                s.EsPrincipal = false;
            }
        }

        sucursal.Nombre = request.Nombre;
        sucursal.Direccion = request.Direccion;
        sucursal.CiudadId = request.CiudadId;
        sucursal.Telefono = request.Telefono;
        sucursal.Email = request.Email;
        sucursal.EsPrincipal = request.EsPrincipal;
        sucursal.Activo = request.Activo;
        sucursal.FechaModificacion = DateTime.UtcNow;
        sucursal.UsuarioModificacion = _currentUserService.UsuarioId;

        // ✅ FIX: La entidad ya está trackeada por _context — SaveChanges la detecta automáticamente
        // No hay que llamar _sucursalRepository.UpdateAsync (evita doble tracking)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sucursal {Id} actualizada", id);
        return Ok(ApiResponse<SucursalDto>.Ok(MapSucursalToDto(sucursal), "Sucursal actualizada exitosamente"));
    }

    /// <summary>
    /// Eliminar sucursal
    /// </summary>
    [HttpDelete("{empresaId}/sucursales/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSucursal(
        long empresaId,
        long id,
        CancellationToken cancellationToken)
    {
        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") &&
            empresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        // ✅ FIX: Buscar con tracking directo desde _context
        var sucursal = await _context.Sucursales
            .FirstOrDefaultAsync(s => s.Id == id && s.EmpresaId == empresaId, cancellationToken);

        if (sucursal == null)
        {
            return NotFound(ApiResponse.Fail("Sucursal no encontrada"));
        }

        if (sucursal.EsPrincipal)
        {
            return BadRequest(ApiResponse.Fail("No se puede eliminar la sucursal principal"));
        }

        sucursal.UsuarioEliminacion = _currentUserService.UsuarioId;

        // ✅ FIX: Eliminar directo desde _context — la entidad ya está trackeada
        _context.Sucursales.Remove(sucursal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sucursal {Id} eliminada", id);
        return Ok(ApiResponse.Ok(message: "Sucursal eliminada exitosamente"));
    }

    #endregion

    #region Mappings

    private static EmpresaDto MapToDto(Empresa empresa) => new()
    {
        Id = empresa.Id,
        Codigo = empresa.Codigo,
        NumeroIdentificacion = empresa.NumeroIdentificacion,
        RazonSocial = empresa.RazonSocial,
        NombreComercial = empresa.NombreComercial,
        Direccion = empresa.DireccionFiscal,
        Telefono = empresa.Telefono,
        Email = empresa.Email,
        SitioWeb = empresa.PaginaWeb,
        LogoUrl = empresa.Logo,
        Activo = empresa.Activo,
        Sucursales = empresa.Sucursales?.Select(MapSucursalToDto).ToList() ?? new()
    };

    private static SucursalDto MapSucursalToDto(Sucursal sucursal) => new()
    {
        Id = sucursal.Id,
        EmpresaId = sucursal.EmpresaId,
        Codigo = sucursal.Codigo,
        Nombre = sucursal.Nombre,
        Direccion = sucursal.Direccion,
        Telefono = sucursal.Telefono,
        Email = sucursal.Email,
        EsPrincipal = sucursal.EsPrincipal,
        Activo = sucursal.Activo
    };

    #endregion
}

#region DTOs

public class EmpresaDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string NumeroIdentificacion { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? SitioWeb { get; set; }
    public string? LogoUrl { get; set; }
    public bool Activo { get; set; }
    public List<SucursalDto> Sucursales { get; set; } = new();
}

public class SucursalDto
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool EsPrincipal { get; set; }
    public bool Activo { get; set; }
}

public class CrearEmpresaRequest
{
    public string Codigo { get; set; } = null!;
    public string TipoIdentificacion { get; set; } = null!;
    public string NumeroIdentificacion { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public long? PaisId { get; set; }
    public string? DireccionFiscal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? PaginaWeb { get; set; }
    public string? Logo { get; set; }
    public long? MonedaPrincipalId { get; set; }
}

public class ActualizarEmpresaRequest
{
    public long Id { get; set; }
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string? DireccionFiscal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? PaginaWeb { get; set; }
    public string? Logo { get; set; }
}

public class CrearSucursalRequest
{
    public string? Codigo { get; set; }   // opcional: se auto-genera si no se envía
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public long? CiudadId { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool EsPrincipal { get; set; }
}

public class ActualizarSucursalRequest
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public long? CiudadId { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool EsPrincipal { get; set; }
    public bool Activo { get; set; }
}

#endregion
