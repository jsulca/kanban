using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IVerificacionRepositorio
{
    PagedResult<Domain.Genericos.Verificacion.Verificacion> ListarPorPagina(VerificacionFiltro? filtro, int page,
        int pageSize);

    List<Domain.Genericos.Verificacion.Verificacion> Listar();

    Domain.Genericos.Verificacion.Verificacion? Buscar(int id);

    bool Guardar(Domain.Genericos.Verificacion.Verificacion entidad);

    void Actualizar(Domain.Genericos.Verificacion.Verificacion entidad);
}