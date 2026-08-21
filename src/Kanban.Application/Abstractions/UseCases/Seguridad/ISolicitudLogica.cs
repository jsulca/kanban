using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface ISolicitudLogica
{
    PagedResult<Solicitud> ListarPorPagina(SolicitudFiltro filter, int pageIndex, int pageSize);

    void Guardar(Solicitud entidad);
}