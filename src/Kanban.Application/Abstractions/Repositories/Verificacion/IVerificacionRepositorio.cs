using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IVerificacionRepositorio
{
    PagedResult<Domain.Genericos.Verificaciones.Verificacion> ListarPorPagina(VerificacionFiltro? filtro, int page,
        int pageSize);

    List<Domain.Genericos.Verificaciones.Verificacion> Listar();

    Domain.Genericos.Verificaciones.Verificacion? Buscar(int id);

    bool Guardar(Domain.Genericos.Verificaciones.Verificacion entidad);

    void Actualizar(Domain.Genericos.Verificaciones.Verificacion entidad);
}