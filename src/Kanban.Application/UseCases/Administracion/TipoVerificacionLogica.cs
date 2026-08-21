using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class TipoVerificacionLogica(ITipoVerificacionRepositorio tipos, ITransacciones transacciones)
    : ITipoVerificacionLogica
{
    public List<TipoVerificacion> Listar(TipoVerificacionFiltro? filtro = null)
    {
        return tipos.Listar(filtro);
    }

    public TipoVerificacion? Buscar(int id)
    {
        return tipos.Buscar(id);
    }

    public bool Guardar(TipoVerificacion entidad)
    {
        return transacciones.Ejecutar(() => tipos.Guardar(entidad));
    }

    public bool Actualizar(TipoVerificacion entidad)
    {
        return transacciones.Ejecutar(() => tipos.Actualizar(entidad));
    }
}