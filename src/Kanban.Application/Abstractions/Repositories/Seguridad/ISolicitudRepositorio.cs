using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface ISolicitudRepositorio
{
    PagedResult<Solicitud> ListarPorPagina(SolicitudFiltro? filter, int page, int pageSize);

    void Guardar(Solicitud entidad);
}