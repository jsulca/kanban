using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface IPaginaLogica
{
    PagedResult<Pagina> ListarPorPagina(PaginaFiltro filter, int pageIndex, int pageSize);

    List<Pagina> Listar(bool conDetalles = false);

    Pagina? BuscarPorId(int id, bool conDetalles = false);

    void Guardar(Pagina entidad);

    void Actualizar(Pagina entidad);
}