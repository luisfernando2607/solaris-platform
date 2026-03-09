using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;

namespace SolarisPlatform.API.Controllers.RRHH;

[Authorize]
[ApiController]
[Route("api/rrhh")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentoService _svc;
    private readonly ICurrentUserService _currentUser;
    public DepartamentosController(IDepartamentoService svc, ICurrentUserService currentUser)
    { _svc = svc; _currentUser = currentUser; }

    [HttpGet("departamentos")]
    public async Task<IActionResult> ObtenerArbol()
        => Ok(await _svc.ObtenerArbolAsync(_currentUser.EmpresaId!.Value));

    [HttpGet("departamentos/{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    { var r = await _svc.ObtenerPorIdAsync(id); return r.Success ? Ok(r) : NotFound(r); }

    [HttpPost("departamentos")]
    public async Task<IActionResult> Crear([FromBody] CrearDepartamentoRequest request)
    { var r = await _svc.CrearAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPut("departamentos/{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarDepartamentoRequest request)
    { var r = await _svc.ActualizarAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpDelete("departamentos/{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    { var r = await _svc.EliminarAsync(id); return r.Success ? Ok(r) : BadRequest(r); }
}

[Authorize]
[ApiController]
[Route("api/rrhh")]
public class PuestosController : ControllerBase
{
    private readonly IPuestoService _svc;
    private readonly ICurrentUserService _currentUser;
    public PuestosController(IPuestoService svc, ICurrentUserService currentUser)
    { _svc = svc; _currentUser = currentUser; }

    [HttpGet("puestos")]
    public async Task<IActionResult> Obtener([FromQuery] long? departamentoId)
        => Ok(await _svc.ObtenerAsync(_currentUser.EmpresaId!.Value, departamentoId));

    [HttpGet("puestos/{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    { var r = await _svc.ObtenerPorIdAsync(id); return r.Success ? Ok(r) : NotFound(r); }

    [HttpPost("puestos")]
    public async Task<IActionResult> Crear([FromBody] CrearPuestoRequest request)
    { var r = await _svc.CrearAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPut("puestos/{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarPuestoRequest request)
    { var r = await _svc.ActualizarAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpDelete("puestos/{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    { var r = await _svc.EliminarAsync(id); return r.Success ? Ok(r) : BadRequest(r); }
}

[Authorize]
[ApiController]
[Route("api/rrhh")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _svc;
    private readonly ICurrentUserService _currentUser;
    public EmpleadosController(IEmpleadoService svc, ICurrentUserService currentUser)
    { _svc = svc; _currentUser = currentUser; }

    [HttpGet("empleados")]
    public async Task<IActionResult> Obtener(
        [FromQuery] int pagina = 1, [FromQuery] int tamano = 20,
        [FromQuery] string? busqueda = null, [FromQuery] long? departamentoId = null,
        [FromQuery] long? puestoId = null, [FromQuery] short? estado = null)
        => Ok(await _svc.ObtenerPaginadoAsync(_currentUser.EmpresaId!.Value, pagina, tamano,
               busqueda, departamentoId, puestoId, estado));

    [HttpGet("empleados/{id:long}")]
    public async Task<IActionResult> ObtenerFicha(long id)
    { var r = await _svc.ObtenerFichaAsync(id); return r.Success ? Ok(r) : NotFound(r); }

    [HttpPost("empleados")]
    public async Task<IActionResult> Crear([FromBody] CrearEmpleadoRequest request)
    { var r = await _svc.CrearAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPut("empleados/{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEmpleadoRequest request)
    { var r = await _svc.ActualizarAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("empleados/{id:long}/egresar")]
    public async Task<IActionResult> Egresar(long id, [FromBody] EgresarEmpleadoRequest request)
    { var r = await _svc.EgresarAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("empleados/{id:long}/cambiar-salario")]
    public async Task<IActionResult> CambiarSalario(long id, [FromBody] CambiarSalarioRequest request)
    { var r = await _svc.CambiarSalarioAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("empleados/{id:long}/cambiar-puesto")]
    public async Task<IActionResult> CambiarPuesto(long id, [FromBody] CambiarPuestoRequest request)
    { var r = await _svc.CambiarPuestoAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("empleados/{id:long}/historial")]
    public async Task<IActionResult> ObtenerHistorial(long id)
        => Ok(await _svc.ObtenerHistorialAsync(id));

    [HttpGet("empleados/{id:long}/documentos")]
    public async Task<IActionResult> ObtenerDocumentos(long id)
        => Ok(await _svc.ObtenerDocumentosAsync(id));

    [HttpPost("empleados/{id:long}/documentos")]
    public async Task<IActionResult> AgregarDocumento(long id, [FromBody] AgregarDocumentoRequest request)
    { var r = await _svc.AgregarDocumentoAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpDelete("empleados/documentos/{documentoId:long}")]
    public async Task<IActionResult> EliminarDocumento(long documentoId)
    { var r = await _svc.EliminarDocumentoAsync(documentoId); return r.Success ? Ok(r) : BadRequest(r); }
}

[Authorize]
[ApiController]
[Route("api/rrhh")]
public class AsistenciaController : ControllerBase
{
    private readonly IAsistenciaService _svc;
    private readonly ICurrentUserService _currentUser;
    public AsistenciaController(IAsistenciaService svc, ICurrentUserService currentUser)
    { _svc = svc; _currentUser = currentUser; }

    [HttpGet("asistencia/empleado/{empleadoId:long}")]
    public async Task<IActionResult> PorEmpleadoMes(long empleadoId, [FromQuery] int anno, [FromQuery] int mes)
        => Ok(await _svc.ObtenerPorEmpleadoMesAsync(empleadoId, anno, mes));

    [HttpPost("asistencia/marcar")]
    public async Task<IActionResult> Marcar([FromBody] MarcarAsistenciaRequest request)
    { var r = await _svc.MarcarAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("ausencias")]
    public async Task<IActionResult> ObtenerAusencias(
        [FromQuery] long? empleadoId = null, [FromQuery] short? estado = null,
        [FromQuery] int pagina = 1, [FromQuery] int tamano = 20)
        => Ok(await _svc.ObtenerAusenciasAsync(_currentUser.EmpresaId!.Value, empleadoId, estado, pagina, tamano));

    [HttpPost("ausencias")]
    public async Task<IActionResult> SolicitarAusencia([FromBody] CrearSolicitudAusenciaRequest request)
    { var r = await _svc.SolicitarAusenciaAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("ausencias/{id:long}/aprobar")]
    public async Task<IActionResult> AprobarAusencia(long id, [FromBody] AprobarRechazarAusenciaRequest request)
    { var r = await _svc.AprobarRechazarAusenciaAsync(id, _currentUser.UsuarioId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("empleados/{empleadoId:long}/vacaciones/{anno:int}")]
    public async Task<IActionResult> SaldoVacaciones(long empleadoId, int anno)
        => Ok(await _svc.ObtenerSaldoVacacionesAsync(empleadoId, anno));
}

[Authorize]
[ApiController]
[Route("api/rrhh")]
public class NominaController : ControllerBase
{
    private readonly INominaService _svc;
    private readonly IConceptoNominaService _conceptoSvc;
    private readonly ICurrentUserService _currentUser;

    public NominaController(INominaService svc, IConceptoNominaService conceptoSvc, ICurrentUserService currentUser)
    { _svc = svc; _conceptoSvc = conceptoSvc; _currentUser = currentUser; }

    [HttpGet("nomina/conceptos")]
    public async Task<IActionResult> ObtenerConceptos()
        => Ok(await _conceptoSvc.ObtenerAsync(_currentUser.EmpresaId!.Value));

    [HttpPost("nomina/conceptos")]
    public async Task<IActionResult> CrearConcepto([FromBody] CrearConceptoNominaRequest request)
    { var r = await _conceptoSvc.CrearAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPut("nomina/conceptos/{id:long}")]
    public async Task<IActionResult> ActualizarConcepto(long id, [FromBody] CrearConceptoNominaRequest request)
    { var r = await _conceptoSvc.ActualizarAsync(id, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpDelete("nomina/conceptos/{id:long}")]
    public async Task<IActionResult> EliminarConcepto(long id)
    { var r = await _conceptoSvc.EliminarAsync(id); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("nomina/periodos")]
    public async Task<IActionResult> ObtenerPeriodos([FromQuery] int anno = 0)
    { if (anno == 0) anno = DateTime.Today.Year; return Ok(await _svc.ObtenerPeriodosAsync(_currentUser.EmpresaId!.Value, anno)); }

    [HttpPost("nomina/periodos")]
    public async Task<IActionResult> CrearPeriodo([FromBody] CrearPeriodoNominaRequest request)
    { var r = await _svc.CrearPeriodoAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("nomina/roles-pago")]
    public async Task<IActionResult> ObtenerRoles([FromQuery] long? periodoId = null, [FromQuery] int pagina = 1, [FromQuery] int tamano = 20)
        => Ok(await _svc.ObtenerRolesPagoAsync(_currentUser.EmpresaId!.Value, periodoId, pagina, tamano));

    [HttpGet("nomina/roles-pago/{id:long}")]
    public async Task<IActionResult> ObtenerRol(long id)
    { var r = await _svc.ObtenerRolPagoAsync(id); return r.Success ? Ok(r) : NotFound(r); }

    [HttpPost("nomina/roles-pago")]
    public async Task<IActionResult> CrearRol([FromBody] CrearRolPagoRequest request)
    { var r = await _svc.CrearRolPagoAsync(_currentUser.EmpresaId!.Value, request); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("nomina/roles-pago/{id:long}/calcular")]
    public async Task<IActionResult> Calcular(long id)
    { var r = await _svc.CalcularRolPagoAsync(id, _currentUser.EmpresaId!.Value); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("nomina/roles-pago/{id:long}/aprobar")]
    public async Task<IActionResult> Aprobar(long id)
    { var r = await _svc.AprobarRolPagoAsync(id, _currentUser.UsuarioId!.Value); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpPost("nomina/roles-pago/{id:long}/marcar-pagado")]
    public async Task<IActionResult> MarcarPagado(long id)
    { var r = await _svc.MarcarPagadoAsync(id, _currentUser.EmpresaId!.Value); return r.Success ? Ok(r) : BadRequest(r); }

    [HttpGet("nomina/roles-pago/{id:long}/detalle")]
    public async Task<IActionResult> ObtenerDetalle(long id)
        => Ok(await _svc.ObtenerDetalleRolAsync(id));

    [HttpGet("nomina/parametros/{paisCodigo}/{anno:int}")]
    public async Task<IActionResult> ObtenerParametros(string paisCodigo, int anno)
        => Ok(await _svc.ObtenerParametrosAsync(_currentUser.EmpresaId!.Value, paisCodigo, anno));

    [HttpPost("nomina/parametros/{paisCodigo}/{anno:int}")]
    public async Task<IActionResult> GuardarParametro(string paisCodigo, int anno, [FromBody] GuardarParametroRequest request)
    { var r = await _svc.GuardarParametroAsync(_currentUser.EmpresaId!.Value, paisCodigo, anno, request); return r.Success ? Ok(r) : BadRequest(r); }
}
