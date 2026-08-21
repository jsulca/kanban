using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IRolPaginaRepositorio
{
    List<RolPagina>? Listar(int rolId);

    void Guardar(RolPagina entidad);

    void Limpiar(int rolId);
}