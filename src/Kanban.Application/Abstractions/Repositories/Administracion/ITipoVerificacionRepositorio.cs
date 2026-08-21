using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface ITipoVerificacionRepositorio
{
    List<TipoVerificacion> Listar(TipoVerificacionFiltro? filtro);

    TipoVerificacion? Buscar(int id);

    bool Guardar(TipoVerificacion entidad);

    bool Actualizar(TipoVerificacion entidad);
}