using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IAreaLogica
{
    List<Area> Listar();

    Area? Buscar(int id);

    bool Guardar(Area entidad);

    bool Actualizar(Area entidad);
}