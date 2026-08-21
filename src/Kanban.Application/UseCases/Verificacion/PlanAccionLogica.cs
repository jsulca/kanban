using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.UseCases.Verificacion;

public class PlanAccionLogica(IPlanAccionRepositorio planes) : IPlanAccionLogica
{
    public PlanAccion? Buscar(int id)
    {
        return planes.Buscar(id);
    }
}