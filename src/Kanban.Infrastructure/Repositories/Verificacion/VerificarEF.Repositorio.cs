using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class VerificarEF : RepositorioGenerico<Verificar>, IVerificarEFRepositorio
{
    public VerificarEF(EFContexto contexto) : base(contexto)
    {
    }
}