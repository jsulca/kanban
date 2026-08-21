using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface IRolLogica
{
    PagedResult<Rol> ListarPorPagina(RolFiltro filter, int pageIndex, int pageSize);

    List<Rol> Listar();

    Rol? BuscarPorId(int id, bool conDetalles = false);

    void Guardar(Rol entidad);

    void Actualizar(Rol entidad);
}