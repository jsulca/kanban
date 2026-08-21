using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IOrigenLogica
{
    List<Origen> Listar(OrigenFiltro? filtro = null);

    Origen? Buscar(int id);

    bool Guardar(Origen entidad);

    bool Actualizar(Origen entidad);
}