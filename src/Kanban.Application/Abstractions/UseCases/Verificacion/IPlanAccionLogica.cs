using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.UseCases.Verificacion;

public interface IPlanAccionLogica
{
    PlanAccion? Buscar(int id);
}