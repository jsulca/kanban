using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IIndicadorRepositorio
{
    List<Indicador> Listar(IndicadorFiltro? filtro);

    Indicador? Buscar(int id);

    bool Guardar(Indicador entidad);

    bool Actualizar(Indicador entidad);
}