using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SolarisPlatform.Application.Common.Mappings;
using SolarisPlatform.Application.Mappings;

namespace SolarisPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper 13+
        services.AddAutoMapper(cfg => 
        {
            cfg.AddProfile<MappingProfile>();
            cfg.AddProfile<EmpleadoMappingProfile>();
            cfg.AddProfile<CatalogoMappingProfile>();
            cfg.AddProfile<ProyectoMappingProfile>();
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<MappingProfile>();

        return services;
    }
}
