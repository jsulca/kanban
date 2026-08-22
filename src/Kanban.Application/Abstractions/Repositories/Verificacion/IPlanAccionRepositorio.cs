using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IPlanAccionRepositorio
{
    List<PlanAccion> Listar(int verificarId);

    PlanAccion? Buscar(int id);

    void Guardar(List<PlanAccion> entidades);
}