using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Verificaciones;

public class PlanAccionEF : RepositorioGenerico<PlanAccion>, IPlanAccionEFRepositorio
{
    public PlanAccionEF(EFContexto contexto) : base(contexto)
    {
    }
}