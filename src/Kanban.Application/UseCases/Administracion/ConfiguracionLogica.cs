using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class ConfiguracionLogica(IConfiguracionRepositorio configuraciones, ITransacciones transacciones)
    : IConfiguracionLogica
{
    public List<Configuracion> Listar()
    {
        return configuraciones.Listar();
    }

    public Configuracion? Buscar(string llave)
    {
        return configuraciones.Buscar(llave);
    }

    public void Actualizar(Configuracion entidad)
    {
        transacciones.Ejecutar(() =>
        {
            configuraciones.Actualizar(entidad);

            // cambiar el plazo de renovación obliga a recalcular los vencimientos
            if (entidad.Llave == ConfiguracionMaestro.RENOVACION_CLAVE)
                configuraciones.ActualizarVencimiento();
        });
    }
}