using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IAreaRepositorio
{
    List<Area> Listar();

    Area? Buscar(int id);

    bool Guardar(Area entidad);

    bool Actualizar(Area entidad);
}