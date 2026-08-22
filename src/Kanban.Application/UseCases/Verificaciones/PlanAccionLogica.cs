using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.UseCases.Verificaciones;

public class PlanAccionLogica(IPlanAccionRepositorio planes) : IPlanAccionLogica
{
    public PlanAccion? Buscar(int id)
    {
        return planes.Buscar(id);
    }
}