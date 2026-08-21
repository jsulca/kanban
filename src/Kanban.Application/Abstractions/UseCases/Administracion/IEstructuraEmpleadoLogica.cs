using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IEstructuraEmpleadoLogica
{
    List<EstructuraEmpleado> Listar(int estructuraId);
}