using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IRolRepositorio
{
    PagedResult<Rol> ListarPorPagina(RolFiltro? filter, int page, int pageSize);

    List<Rol>? Listar();

    Rol? BuscarPorId(int id);

    void Guardar(Rol entidad);

    void Actualizar(Rol entidad);
}