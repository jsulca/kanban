using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IRolControlRepositorio
{
    List<RolControl>? Listar(int rolId);

    void Guardar(RolControl entidad);

    void Limpiar(int rolId);
}