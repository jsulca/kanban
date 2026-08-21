using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class AreaLogica(IAreaRepositorio areas, ITransacciones transacciones) : IAreaLogica
{
    public List<Area> Listar()
    {
        return areas.Listar();
    }

    public Area? Buscar(int id)
    {
        return areas.Buscar(id);
    }

    public bool Guardar(Area entidad)
    {
        return transacciones.Ejecutar(() => areas.Guardar(entidad));
    }

    public bool Actualizar(Area entidad)
    {
        return transacciones.Ejecutar(() => areas.Actualizar(entidad));
    }
}