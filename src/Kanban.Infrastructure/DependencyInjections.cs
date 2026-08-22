using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Infrastructure.Common;
using Kanban.Infrastructure.Context;
using Kanban.Infrastructure.Repositories;
using Kanban.Infrastructure.Repositories.Administracion;
using Kanban.Infrastructure.Repositories.Compromisos;
using Kanban.Infrastructure.Repositories.Seguridad;
using Kanban.Infrastructure.Repositories.Verificaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Infrastructure;

public static class DependencyInjections
{
    public const string ConnectionStringName = "Kanban";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
                               ?? throw new InvalidOperationException(
                                   $"Falta la cadena de conexión '{ConnectionStringName}' en la configuración.");

        // La conexión se registra como Scoped para que todos los repositorios de una
        // misma petición compartan una sola conexión (y puedan compartir transacción).
        services.AddNpgsqlDataSource(connectionString, connectionLifetime: ServiceLifetime.Scoped);

        services.AddDbContext<EFContexto>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Transacciones sobre la conexión ADO compartida del ámbito. Ojo: no cubre
        // los cambios de EFContexto, que abre su propia conexión.
        services.AddScoped<ITransacciones, Transacciones>();

        return services
            .AddRepositoriosAdministracion()
            .AddRepositoriosCompromiso()
            .AddRepositoriosSeguridad()
            .AddRepositoriosVerificacion();
    }

    private static IServiceCollection AddRepositoriosAdministracion(this IServiceCollection services)
    {
        services.AddScoped<IAdjuntoRepositorio, AdjuntoRepositorio>();
        services.AddScoped<IAreaRepositorio, AreaRepositorio>();
        services.AddScoped<ICargoRepositorio, CargoRepositorio>();
        services.AddScoped<IColorRepositorio, ColorRepositorio>();
        services.AddScoped<IConfiguracionRepositorio, ConfiguracionRepositorio>();
        services.AddScoped<IEmpleadoRepositorio, EmpleadoRepositorio>();
        services.AddScoped<IEstructuraRepositorio, EstructuraRepositorio>();
        services.AddScoped<IEstructuraAreaRepositorio, EstructuraAreaRepositorio>();
        services.AddScoped<IEstructuraEmpleadoRepositorio, EstructuraEmpleadoRepositorio>();
        services.AddScoped<IEstructuraInstanciaRepositorio, EstructuraInstanciaRepositorio>();
        services.AddScoped<IIndicadorRepositorio, IndicadorRepositorio>();
        services.AddScoped<IInstanciaRepositorio, InstanciaRepositorio>();
        services.AddScoped<IOrigenRepositorio, OrigenRepositorio>();
        services.AddScoped<ISostenibilidadRepositorio, SostenibilidadRepositorio>();
        services.AddScoped<ITipoVerificacionRepositorio, TipoVerificacionRepositorio>();

        services.AddScoped<IAdjuntoEFRepositorio, AdjuntoEF>();
        services.AddScoped<ICargoEFRepositorio, CargoEF>();
        services.AddScoped<IEmpleadoEFRepositorio, EmpleadoEF>();
        services.AddScoped<IEstructuraEFRepositorio, EstructuraEF>();

        return services;
    }

    private static IServiceCollection AddRepositoriosCompromiso(this IServiceCollection services)
    {
        services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();
        services.AddScoped<ICompromisoRepositorio, CompromisoRepositorio>();
        services.AddScoped<ICompromisoEstadoRepositorio, CompromisoEstadoRepositorio>();
        services.AddScoped<ICompromisoInstanciaRepositorio, CompromisoInstanciaRepositorio>();

        return services;
    }

    private static IServiceCollection AddRepositoriosSeguridad(this IServiceCollection services)
    {
        services.AddScoped<IControlRepositorio, ControlRepositorio>();
        services.AddScoped<IIntentoRepositorio, IntentoRepositorio>();
        services.AddScoped<IMenuRepositorio, MenuRepositorio>();
        services.AddScoped<IPaginaRepositorio, PaginaRepositorio>();
        services.AddScoped<IRolRepositorio, RolRepositorio>();
        services.AddScoped<IRolControlRepositorio, RolControlRepositorio>();
        services.AddScoped<IRolMenuRepositorio, RolMenuRepositorio>();
        services.AddScoped<IRolPaginaRepositorio, RolPaginaRepositorio>();
        services.AddScoped<ISolicitudRepositorio, SolicitudRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IUsuarioEstructuraRepositorio, UsuarioEstructuraRepositorio>();

        services.AddScoped<IIntentoEFRepositorio, IntentoEF>();
        services.AddScoped<IUsuarioEFRepositorio, UsuarioEF>();
        services.AddScoped<IUsuarioEstructuraEFRepositorio, UsuarioEstructuraEF>();

        return services;
    }

    private static IServiceCollection AddRepositoriosVerificacion(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<IConfirmadorComentarioRepositorio, ConfirmadorComentarioRepositorio>();
        services.AddScoped<IConfirmadorSemanaRepositorio, ConfirmadorSemanaRepositorio>();
        services.AddScoped<IPlanAccionRepositorio, PlanAccionRepositorio>();
        services.AddScoped<IPreguntaRepositorio, PreguntaRepositorio>();
        services.AddScoped<IRespuestaRepositorio, RespuestaRepositorio>();
        services.AddScoped<ISostenibilidadMesRepositorio, SostenibilidadMesRepositorio>();
        services.AddScoped<IVerificacionRepositorio, VerificacionRepositorio>();
        services.AddScoped<IVerificarRepositorio, VerificarRepositorio>();
        services.AddScoped<IVerificarRespuestaRepositorio, VerificarRespuestaRepositorio>();

        services.AddScoped<IPlanAccionEFRepositorio, PlanAccionEF>();
        services.AddScoped<IVerificarEFRepositorio, VerificarEF>();
        services.AddScoped<IVerificarRespuestaEFRepositorio, VerificarRespuestaEF>();

        return services;
    }
}
