using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IVerificarRepositorio
{
    PagedResult<Verificar> ListarPorPagina(VerificarFiltro? filter, int page, int pageSize);

    List<Verificar> TableroResumen(VerificarFiltro filter);

    List<Verificar> Reporte(int tableroId, DateTime fechaDesde, DateTime fechaHasta);

    Verificar? Buscar(int id);

    bool Guardar(Verificar entidad);
}