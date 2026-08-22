using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Verificaciones;

public class VerificarEF : RepositorioGenerico<Verificar>, IVerificarEFRepositorio
{
    public VerificarEF(EFContexto contexto) : base(contexto)
    {
    }
}