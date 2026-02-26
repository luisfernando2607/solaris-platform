using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SolarisPlatform.Application.Common.Mappings;

namespace SolarisPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper 13+
        services.AddAutoMapper(cfg => 
        {
            cfg.AddProfile<MappingProfile>();
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<MappingProfile>();

        return services;
    }
}
