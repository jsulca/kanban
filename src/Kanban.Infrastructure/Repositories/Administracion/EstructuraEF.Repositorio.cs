using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EstructuraEF : RepositorioGenerico<Estructura>, IEstructuraEFRepositorio
{
    public EstructuraEF(EFContexto contexto) : base(contexto)
    {
    }
}