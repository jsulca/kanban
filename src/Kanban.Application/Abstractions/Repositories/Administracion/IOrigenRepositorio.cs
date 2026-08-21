using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IOrigenRepositorio
{
    List<Origen> Listar(OrigenFiltro? filtro);

    Origen? Buscar(int id);

    bool Guardar(Origen entidad);

    bool Actualizar(Origen entidad);
}