using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IRolMenuRepositorio
{
    List<RolMenu>? Listar(int rolId);

    void Guardar(RolMenu entidad);

    void Limpiar(int rolId);
}