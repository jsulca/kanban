using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IEmpleadoRepositorio
{
    List<Empleado> Listar(EmpleadoFiltro? filtro);

    Empleado? Buscar(int id);

    bool Guardar(Empleado entidad);

    void Guardar(List<Empleado> entidades);

    bool Actualizar(Empleado entidad);
}