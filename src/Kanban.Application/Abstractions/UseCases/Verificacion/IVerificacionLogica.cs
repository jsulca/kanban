using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.UseCases.Verificacion;

public interface IVerificacionLogica
{
    PagedResult<Domain.Genericos.Verificacion.Verificacion> ListarPorPagina(VerificacionFiltro filtro, int page,
        int pageSize);

    List<Domain.Genericos.Verificacion.Verificacion> Listar();

    Domain.Genericos.Verificacion.Verificacion? Buscar(int id, bool conDetalles = false);

    void Guardar(Domain.Genericos.Verificacion.Verificacion entidad);

    void Actualizar(Domain.Genericos.Verificacion.Verificacion entidad);
}