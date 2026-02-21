using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SolarisPlatform.Application.Common.Interfaces;

namespace SolarisPlatform.Infrastructure.Identity;

/// <summary>
/// Servicio para obtener información del usuario actual autenticado
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UsuarioId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
            return long.TryParse(userId, out var id) ? id : null;
        }
    }

    public long? EmpresaId
    {
        get
        {
            var empresaId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("empresaId");
            return long.TryParse(empresaId, out var id) ? id : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles
    {
        get
        {
            var roles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            return roles?.Select(r => r.Value) ?? Enumerable.Empty<string>();
        }
    }

    public IEnumerable<string> Permisos
    {
        get
        {
            var permisos = _httpContextAccessor.HttpContext?.User?.FindAll("permiso");
            return permisos?.Select(p => p.Value) ?? Enumerable.Empty<string>();
        }
    }

    public bool TienePermiso(string permiso)
    {
        return Permisos.Contains(permiso);
    }

    public bool TieneRol(string rol)
    {
        return Roles.Contains(rol);
    }
}
