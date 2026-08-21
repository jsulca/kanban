using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface ITipoVerificacionLogica
{
    List<TipoVerificacion> Listar(TipoVerificacionFiltro? filtro = null);

    TipoVerificacion? Buscar(int id);

    bool Guardar(TipoVerificacion entidad);

    bool Actualizar(TipoVerificacion entidad);
}