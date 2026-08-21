using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IPaginaRepositorio
{
    PagedResult<Pagina> ListarPorPagina(PaginaFiltro? filter, int page, int pageSize);

    List<Pagina>? Listar();

    Pagina? Buscar(int id);

    void Guardar(Pagina entidad);

    void Actualizar(Pagina entidad);
}