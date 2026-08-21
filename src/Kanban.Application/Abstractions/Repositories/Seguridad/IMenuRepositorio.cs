using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IMenuRepositorio
{
    List<Menu>? Listar();

    Menu? Buscar(int id);

    void Guardar(Menu entidad);

    void Actualizar(Menu entidad);
}