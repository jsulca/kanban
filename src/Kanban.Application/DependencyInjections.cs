using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Application.UseCases.Administracion;
using Kanban.Application.UseCases.Compromiso;
using Kanban.Application.UseCases.Seguridad;
using Kanban.Application.UseCases.Verificacion;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Application;

public static class DependencyInjections
{
    /// <summary>
    ///     Registra las Lógicas. Dependen de las interfaces de repositorio y de
    ///     <c>ITransacciones</c>, que aporta <c>AddInfrastructure</c>: hay que llamar a
    ///     los dos.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddLogicasAdministracion()
            .AddLogicasCompromiso()
            .AddLogicasSeguridad()
            .AddLogicasVerificacion();
    }

    private static IServiceCollection AddLogicasAdministracion(this IServiceCollection services)
    {
        services.AddScoped<IAdjuntoLogica, AdjuntoLogica>();
        services.AddScoped<IAreaLogica, AreaLogica>();
        services.AddScoped<ICargoLogica, CargoLogica>();
        services.AddScoped<IColorLogica, ColorLogica>();
        services.AddScoped<IConfiguracionLogica, ConfiguracionLogica>();
        services.AddScoped<IEmpleadoLogica, EmpleadoLogica>();
        services.AddScoped<IEstructuraLogica, EstructuraLogica>();
        services.AddScoped<IEstructuraAreaLogica, EstructuraAreaLogica>();
        services.AddScoped<IEstructuraEmpleadoLogica, EstructuraEmpleadoLogica>();
        services.AddScoped<IEstructuraInstanciaLogica, EstructuraInstanciaLogica>();
        services.AddScoped<IIndicadorLogica, IndicadorLogica>();
        services.AddScoped<IInstanciaLogica, InstanciaLogica>();
        services.AddScoped<IOrigenLogica, OrigenLogica>();
        services.AddScoped<ISostenibilidadLogica, SostenibilidadLogica>();
        services.AddScoped<ITipoVerificacionLogica, TipoVerificacionLogica>();

        return services;
    }

    private static IServiceCollection AddLogicasCompromiso(this IServiceCollection services)
    {
        services.AddScoped<IAlertaLogica, AlertaLogica>();
        services.AddScoped<ICompromisoLogica, CompromisoLogica>();

        return services;
    }

    private static IServiceCollection AddLogicasSeguridad(this IServiceCollection services)
    {
        services.AddScoped<IIntentoLogica, IntentoLogica>();
        services.AddScoped<IMenuLogica, MenuLogica>();
        services.AddScoped<IPaginaLogica, PaginaLogica>();
        services.AddScoped<IRolLogica, RolLogica>();
        services.AddScoped<ISolicitudLogica, SolicitudLogica>();
        services.AddScoped<IUsuarioLogica, UsuarioLogica>();

        return services;
    }

    private static IServiceCollection AddLogicasVerificacion(this IServiceCollection services)
    {
        services.AddScoped<IPlanAccionLogica, PlanAccionLogica>();
        services.AddScoped<IVerificacionLogica, VerificacionLogica>();
        services.AddScoped<IVerificarLogica, VerificarLogica>();

        return services;
    }
}