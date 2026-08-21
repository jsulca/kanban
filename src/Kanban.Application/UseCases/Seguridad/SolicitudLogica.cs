using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class SolicitudLogica(ISolicitudRepositorio solicitudes, ITransacciones transacciones)
    : ISolicitudLogica
{
    public PagedResult<Solicitud> ListarPorPagina(SolicitudFiltro filter, int pageIndex, int pageSize)
    {
        return solicitudes.ListarPorPagina(filter, pageIndex, pageSize);
    }

    public void Guardar(Solicitud entidad)
    {
        // la versión 4.8 construía aquí un UsuarioEstructuraRepositorio que no llegaba a usar
        transacciones.Ejecutar(() => solicitudes.Guardar(entidad));
    }
}