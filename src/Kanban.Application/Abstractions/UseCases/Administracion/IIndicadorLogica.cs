using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IIndicadorLogica
{
    List<Indicador> Listar(IndicadorFiltro? filtro = null);

    Indicador? Buscar(int id);

    bool Guardar(Indicador entidad);

    bool Actualizar(Indicador entidad);
}