using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Domain.Entities.RRHH;
using SolarisPlatform.Domain.Interfaces.RRHH;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

/// <summary>
/// Servicio de empleados
/// </summary>
public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public EmpleadoService(IEmpleadoRepository repo, ICurrentUserService currentUser, IMapper mapper)
    { _repo = repo; _currentUser = currentUser; _mapper = mapper; }

    public async Task<ApiResponse<PaginatedList<EmpleadoListaDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null)
    {
        var (items, total) = await _repo.ObtenerPaginadoAsync(empresaId, pagina, tamano, busqueda, departamentoId, puestoId, estado);
        var dtos = _mapper.Map<IEnumerable<EmpleadoListaDto>>(items);
        return ApiResponse<PaginatedList<EmpleadoListaDto>>.Ok(
            new PaginatedList<EmpleadoListaDto>(dtos.ToList(), total, pagina, tamano));
    }

    public async Task<ApiResponse<EmpleadoFichaDto>> ObtenerFichaAsync(long id)
    {
        var emp = await _repo.ObtenerFichaCompletaAsync(id);
        if (emp == null) return ApiResponse<EmpleadoFichaDto>.Fail("Empleado no encontrado");
        return ApiResponse<EmpleadoFichaDto>.Ok(_mapper.Map<EmpleadoFichaDto>(emp));
    }

    public async Task<ApiResponse<EmpleadoFichaDto>> CrearAsync(long empresaId, CrearEmpleadoRequest request)
    {
        if (await _repo.ExisteIdentificacionAsync(empresaId, request.TipoIdentificacion, request.NumeroIdentificacion))
            return ApiResponse<EmpleadoFichaDto>.Fail("Ya existe un empleado con esa identificación");

        var emp = _mapper.Map<Empleado>(request);
        emp.EmpresaId = empresaId;
        emp.Estado = 1;
        // Generar código automático basado en el ID (se actualiza post-creación)

        var historial = new EmpleadoHistorial
        {
            EmpresaId     = empresaId,
            TipoCambio    = 1,
            FechaEfectiva = request.FechaIngreso,
            SalarioNuevo  = request.SalarioBase
        };
        emp.Historial.Add(historial);

        var creado = await _repo.CrearAsync(emp);
        return ApiResponse<EmpleadoFichaDto>.Ok(_mapper.Map<EmpleadoFichaDto>(creado));
    }

    public async Task<ApiResponse<EmpleadoFichaDto>> ActualizarAsync(long id, ActualizarEmpleadoRequest request)
    {
        var emp = await _repo.ObtenerPorIdAsync(id);
        if (emp == null) return ApiResponse<EmpleadoFichaDto>.Fail("Empleado no encontrado");

        _mapper.Map(request, emp);
        var actualizado = await _repo.ActualizarAsync(emp);
        return ApiResponse<EmpleadoFichaDto>.Ok(_mapper.Map<EmpleadoFichaDto>(actualizado));
    }

    public async Task<ApiResponse> EgresarAsync(long id, EgresarEmpleadoRequest request)
    {
        var emp = await _repo.ObtenerPorIdAsync(id);
        if (emp == null) return ApiResponse.Fail("Empleado no encontrado");
        if (emp.Estado == 4) return ApiResponse.Fail("El empleado ya está egresado");

        var salarioAnterior = emp.SalarioBase;
        emp.Estado = 4;
        emp.FechaEgreso = request.FechaEgreso;
        emp.MotivoEgreso = request.MotivoEgreso;
        emp.DescripcionEgreso = request.Descripcion;
        await _repo.ActualizarAsync(emp);

        await _repo.RegistrarHistorialAsync(new EmpleadoHistorial
        {
            EmpleadoId = id,
            TipoCambio = 6,
            FechaEfectiva = request.FechaEgreso,
            SalarioAnterior = salarioAnterior,
            Motivo = request.Descripcion
        });
        return ApiResponse.Ok();
    }

    public async Task<ApiResponse> CambiarSalarioAsync(long id, CambiarSalarioRequest request)
    {
        var emp = await _repo.ObtenerPorIdAsync(id);
        if (emp == null) return ApiResponse.Fail("Empleado no encontrado");

        var salarioAnterior = emp.SalarioBase;
        emp.SalarioBase = request.NuevoSalario;
        await _repo.ActualizarAsync(emp);

        await _repo.RegistrarHistorialAsync(new EmpleadoHistorial
        {
            EmpleadoId = id,
            TipoCambio = 3,
            FechaEfectiva = request.FechaEfectiva,
            SalarioAnterior = salarioAnterior,
            SalarioNuevo = request.NuevoSalario,
            Motivo = request.Motivo
        });
        return ApiResponse.Ok();
    }

    public async Task<ApiResponse> CambiarPuestoAsync(long id, CambiarPuestoRequest request)
    {
        var emp = await _repo.ObtenerPorIdAsync(id);
        if (emp == null) return ApiResponse.Fail("Empleado no encontrado");

        emp.PuestoId = request.NuevoPuestoId;
        if (request.NuevoDepartamentoId.HasValue) emp.DepartamentoId = request.NuevoDepartamentoId;
        await _repo.ActualizarAsync(emp);

        await _repo.RegistrarHistorialAsync(new EmpleadoHistorial
        {
            EmpleadoId = id,
            TipoCambio = 2,
            FechaEfectiva = request.FechaEfectiva,
            Motivo = request.Motivo
        });
        return ApiResponse.Ok();
    }

    public async Task<ApiResponse<IEnumerable<EmpleadoHistorialDto>>> ObtenerHistorialAsync(long id)
    {
        var historial = await _repo.ObtenerHistorialAsync(id);
        return ApiResponse<IEnumerable<EmpleadoHistorialDto>>.Ok(_mapper.Map<IEnumerable<EmpleadoHistorialDto>>(historial));
    }

    public async Task<ApiResponse<IEnumerable<EmpleadoDocumentoDto>>> ObtenerDocumentosAsync(long id)
    {
        var docs = await _repo.ObtenerDocumentosAsync(id);
        return ApiResponse<IEnumerable<EmpleadoDocumentoDto>>.Ok(_mapper.Map<IEnumerable<EmpleadoDocumentoDto>>(docs));
    }

    public async Task<ApiResponse<EmpleadoDocumentoDto>> AgregarDocumentoAsync(long id, AgregarDocumentoRequest request)
    {
        var doc = _mapper.Map<EmpleadoDocumento>(request);
        doc.EmpleadoId = id;
        var creado = await _repo.AgregarDocumentoAsync(doc);
        return ApiResponse<EmpleadoDocumentoDto>.Ok(_mapper.Map<EmpleadoDocumentoDto>(creado));
    }

    public async Task<ApiResponse> EliminarDocumentoAsync(long documentoId)
    {
        await _repo.EliminarDocumentoAsync(documentoId);
        return ApiResponse.Ok();
    }
}

/// <summary>
/// Servicio de nómina: períodos, roles de pago, cálculo
/// </summary>
public class NominaService : INominaService
{
    private readonly IRolPagoRepository _rolRepo;
    private readonly IPeriodoNominaRepository _periodoRepo;
    private readonly IConceptoNominaRepository _conceptoRepo;
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IParametroNominaRepository _paramRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public NominaService(IRolPagoRepository rolRepo, IPeriodoNominaRepository periodoRepo,
        IConceptoNominaRepository conceptoRepo, IEmpleadoRepository empleadoRepo,
        IParametroNominaRepository paramRepo, ICurrentUserService currentUser, IMapper mapper)
    {
        _rolRepo = rolRepo; _periodoRepo = periodoRepo; _conceptoRepo = conceptoRepo;
        _empleadoRepo = empleadoRepo; _paramRepo = paramRepo;
        _currentUser = currentUser; _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<PeriodoNominaDto>>> ObtenerPeriodosAsync(long empresaId, int anno)
    {
        var periodos = await _periodoRepo.ObtenerPorEmpresaAnnoAsync(empresaId, anno);
        return ApiResponse<IEnumerable<PeriodoNominaDto>>.Ok(_mapper.Map<IEnumerable<PeriodoNominaDto>>(periodos));
    }

    public async Task<ApiResponse<PeriodoNominaDto>> CrearPeriodoAsync(long empresaId, CrearPeriodoNominaRequest request)
    {
        var periodo = _mapper.Map<PeriodoNomina>(request);
        periodo.EmpresaId = empresaId;
        periodo.Estado = 1;
        var creado = await _periodoRepo.CrearAsync(periodo);
        return ApiResponse<PeriodoNominaDto>.Ok(_mapper.Map<PeriodoNominaDto>(creado));
    }

    public async Task<ApiResponse<IEnumerable<RolPagoDto>>> ObtenerRolesPagoAsync(long empresaId, long? periodoId = null, int pagina = 1, int tamano = 20)
    {
        var (items, _) = await _rolRepo.ObtenerPaginadoAsync(empresaId, pagina, tamano, periodoId);
        return ApiResponse<IEnumerable<RolPagoDto>>.Ok(_mapper.Map<IEnumerable<RolPagoDto>>(items));
    }

    public async Task<ApiResponse<RolPagoDto>> ObtenerRolPagoAsync(long id)
    {
        var rol = await _rolRepo.ObtenerPorIdAsync(id);
        if (rol == null) return ApiResponse<RolPagoDto>.Fail("Rol de pago no encontrado");
        return ApiResponse<RolPagoDto>.Ok(_mapper.Map<RolPagoDto>(rol));
    }

    public async Task<ApiResponse<RolPagoDto>> CrearRolPagoAsync(long empresaId, CrearRolPagoRequest request)
    {
        var rol = new RolPago
        {
            EmpresaId = empresaId,
            PeriodoId = request.PeriodoId,
            Numero = await _rolRepo.GenerarNumeroAsync(empresaId),
            Tipo = request.Tipo,
            Descripcion = request.Descripcion,
            Estado = 1
        };
        var creado = await _rolRepo.CrearAsync(rol);
        return ApiResponse<RolPagoDto>.Ok(_mapper.Map<RolPagoDto>(creado));
    }

    public async Task<ApiResponse<RolPagoDto>> CalcularRolPagoAsync(long id, long empresaId)
    {
        var rol = await _rolRepo.ObtenerConDetalleAsync(id);
        if (rol == null) return ApiResponse<RolPagoDto>.Fail("Rol de pago no encontrado");
        if (rol.Estado != 1) return ApiResponse<RolPagoDto>.Fail("Solo se puede calcular un rol en estado Borrador");

        var conceptos = (await _conceptoRepo.ObtenerPorEmpresaAsync(empresaId))
            .OrderBy(c => c.OrdenCalculo).ToList();

        var (empleados, _) = await _empleadoRepo.ObtenerPaginadoAsync(empresaId, 1, 9999, estado: 1);
        var periodo = await _periodoRepo.ObtenerPorIdAsync(rol.PeriodoId);

        decimal totalIngresos = 0, totalDescuentos = 0, totalAportes = 0;

        foreach (var emp in empleados)
        {
            var rolEmp = new RolPagoEmpleado
            {
                RolPagoId = id,
                EmpleadoId = emp.Id,
                EmpresaId = empresaId,
                PuestoNombre = emp.Puesto?.Nombre,
                DepartamentoNombre = emp.Departamento?.Nombre,
                SalarioBase = emp.SalarioBase,
                DiasTrabajados = periodo != null
                    ? (decimal)(periodo.FechaFin.DayNumber - periodo.FechaInicio.DayNumber + 1)
                    : 30m,
                HorasExtras = 0,
                EstadoPago = 1
            };

            var detalles = new List<RolPagoDetalle>();
            decimal ingresos = emp.SalarioBase;
            decimal descuentos = 0;
            decimal aportes = 0;
            var calculados = new Dictionary<long, decimal> { [0] = emp.SalarioBase };

            foreach (var concepto in conceptos)
            {
                decimal valor = concepto.FormaCalculo switch
                {
                    1 => concepto.ValorFijo ?? 0,
                    2 => emp.SalarioBase * (concepto.Porcentaje ?? 0) / 100,
                    3 => concepto.ConceptoBaseId.HasValue && calculados.ContainsKey(concepto.ConceptoBaseId.Value)
                        ? calculados[concepto.ConceptoBaseId.Value] * (concepto.Porcentaje ?? 0) / 100
                        : 0,
                    _ => 0
                };
                valor = Math.Round(valor, 2);
                calculados[concepto.Id] = valor;

                detalles.Add(new RolPagoDetalle
                {
                    ConceptoId = concepto.Id,
                    EmpresaId = empresaId,
                    BaseCalculo = concepto.FormaCalculo == 2 ? emp.SalarioBase : null,
                    PorcentajeAplicado = concepto.Porcentaje,
                    Cantidad = 1,
                    Valor = valor,
                    Tipo = concepto.Tipo
                });

                if (concepto.Tipo == 1) ingresos += valor;
                else if (concepto.Tipo == 2) descuentos += valor;
                else if (concepto.Tipo == 3) aportes += valor;
            }

            rolEmp.TotalIngresos = ingresos;
            rolEmp.TotalDescuentos = descuentos;
            rolEmp.TotalAportesPatronales = aportes;
            rolEmp.NetoAPagar = ingresos - descuentos;

            var empCreado = await _rolRepo.CrearEmpleadoAsync(rolEmp);
            foreach (var d in detalles) d.RolPagoEmpleadoId = empCreado.Id;
            await _rolRepo.GuardarDetallesAsync(detalles);

            totalIngresos += ingresos;
            totalDescuentos += descuentos;
            totalAportes += aportes;
        }

        rol.TotalIngresos = totalIngresos;
        rol.TotalDescuentos = totalDescuentos;
        rol.TotalAportesPatronales = totalAportes;
        rol.TotalNeto = totalIngresos - totalDescuentos;
        rol.TotalEmpleados = empleados.Count();
        rol.Estado = 2; // Calculado

        var actualizado = await _rolRepo.ActualizarAsync(rol);
        return ApiResponse<RolPagoDto>.Ok(_mapper.Map<RolPagoDto>(actualizado));
    }

    public async Task<ApiResponse<RolPagoDto>> AprobarRolPagoAsync(long id, long aprobadorId)
    {
        var rol = await _rolRepo.ObtenerPorIdAsync(id);
        if (rol == null) return ApiResponse<RolPagoDto>.Fail("Rol no encontrado");
        if (rol.Estado != 2) return ApiResponse<RolPagoDto>.Fail("Solo se puede aprobar un rol en estado Calculado");

        rol.Estado = 3;
        rol.AprobadoPorId = aprobadorId;
        rol.FechaAprobacion = DateTime.UtcNow;
        await _rolRepo.ActualizarAsync(rol);
        return ApiResponse<RolPagoDto>.Ok(_mapper.Map<RolPagoDto>(rol));
    }

    public async Task<ApiResponse> MarcarPagadoAsync(long id, long empresaId)
    {
        var rol = await _rolRepo.ObtenerPorIdAsync(id);
        if (rol == null) return ApiResponse.Fail("Rol no encontrado");
        if (rol.Estado != 3) return ApiResponse.Fail("Solo se puede marcar pagado un rol aprobado");
        rol.Estado = 4;
        await _rolRepo.ActualizarAsync(rol);
        return ApiResponse.Ok();
    }

    public async Task<ApiResponse<IEnumerable<RolPagoEmpleadoDto>>> ObtenerDetalleRolAsync(long rolPagoId)
    {
        var rol = await _rolRepo.ObtenerConDetalleAsync(rolPagoId);
        if (rol == null) return ApiResponse<IEnumerable<RolPagoEmpleadoDto>>.Fail("Rol no encontrado");
        return ApiResponse<IEnumerable<RolPagoEmpleadoDto>>.Ok(_mapper.Map<IEnumerable<RolPagoEmpleadoDto>>(rol.Empleados));
    }

    public async Task<ApiResponse<IEnumerable<ParametroNominaDto>>> ObtenerParametrosAsync(long empresaId, string paisCodigo, int anno)
    {
        var params_ = await _paramRepo.ObtenerAsync(empresaId, paisCodigo, anno);
        return ApiResponse<IEnumerable<ParametroNominaDto>>.Ok(_mapper.Map<IEnumerable<ParametroNominaDto>>(params_));
    }

    public async Task<ApiResponse<ParametroNominaDto>> GuardarParametroAsync(long empresaId, string paisCodigo, int anno, GuardarParametroRequest request)
    {
        var param = new ParametroNomina
        {
            EmpresaId = empresaId,
            PaisCodigo = paisCodigo,
            Anno = anno,
            Clave = request.Clave,
            Valor = request.Valor,
            Descripcion = request.Descripcion,
            TipoDato = request.TipoDato
        };
        var guardado = await _paramRepo.CrearOActualizarAsync(param);
        return ApiResponse<ParametroNominaDto>.Ok(_mapper.Map<ParametroNominaDto>(guardado));
    }
}
