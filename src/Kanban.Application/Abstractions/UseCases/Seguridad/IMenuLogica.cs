using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface IMenuLogica
{
    List<Menu> Listar();

    Menu? BuscarPorId(int id);

    void Guardar(Menu entidad);

    void Actualizar(Menu entidad);
}