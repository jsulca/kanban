using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.UseCases.Verificacion;

public interface IPlanAccionLogica
{
    PlanAccion? Buscar(int id);
}