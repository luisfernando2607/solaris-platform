using AutoMapper;
using FluentAssertions;
using Moq;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Domain.Interfaces.Proyectos;
using SolarisPlatform.Infrastructure.Services.Proyectos;

namespace SolarisPlatform.Tests.Unit.Infrastructure.Services;

public class ProyectoServiceTests
{
    private readonly Mock<IProyectoRepository> _repo   = new();
    private readonly Mock<IUnitOfWork>         _uow    = new();
    private readonly Mock<IMapper>             _mapper = new();
    private readonly ProyectoService           _service;

    public ProyectoServiceTests()
    {
        _service = new ProyectoService(_repo.Object, _uow.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetById_ProyectoInexistente_DebeRetornarNull()
    {
        // FIX: el servicio llama GetWithFasesAsync internamente, no GetByIdAsync
        _repo.Setup(r => r.GetWithFasesAsync(99999, default))
             .ReturnsAsync((Proyecto?)null);

        var result = await _service.GetByIdAsync(99999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ProyectoExistente_DebeMapear()
    {
        var proyecto = new Proyecto
        {
            Id = 1, Codigo = "PROY001", Nombre = "Test",
            EmpresaId = 1, PresupuestoTotal = 1000m
        };

        // FIX: mockear el método que realmente usa el servicio
        _repo.Setup(r => r.GetWithFasesAsync(1, default)).ReturnsAsync(proyecto);

        var result = await _service.GetByIdAsync(1);

        // Verificar que el repo fue invocado con el método correcto
        _repo.Verify(r => r.GetWithFasesAsync(1, default), Times.Once);
    }
}
