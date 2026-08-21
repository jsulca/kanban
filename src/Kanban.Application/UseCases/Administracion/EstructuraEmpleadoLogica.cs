using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class EstructuraEmpleadoLogica(IEstructuraEmpleadoRepositorio empleados) : IEstructuraEmpleadoLogica
{
    public List<EstructuraEmpleado> Listar(int estructuraId)
    {
        return empleados.Listar(estructuraId);
    }
}