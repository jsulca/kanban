using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IEstructuraEmpleadoRepositorio
{
    List<EstructuraEmpleado> Listar(int estructuraId);

    void Guardar(EstructuraEmpleado entidad);

    void Guardar(List<EstructuraEmpleado> entidades);

    bool Limpiar(int estructuraId);
}