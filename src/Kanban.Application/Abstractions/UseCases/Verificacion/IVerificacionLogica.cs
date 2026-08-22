using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.UseCases.Verificacion;

public interface IVerificacionLogica
{
    PagedResult<Domain.Genericos.Verificaciones.Verificacion> ListarPorPagina(VerificacionFiltro filtro, int page,
        int pageSize);

    List<Domain.Genericos.Verificaciones.Verificacion> Listar();

    Domain.Genericos.Verificaciones.Verificacion? Buscar(int id, bool conDetalles = false);

    void Guardar(Domain.Genericos.Verificaciones.Verificacion entidad);

    void Actualizar(Domain.Genericos.Verificaciones.Verificacion entidad);
}