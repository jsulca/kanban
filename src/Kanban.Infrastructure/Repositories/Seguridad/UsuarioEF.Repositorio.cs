using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class UsuarioEF : RepositorioGenerico<Usuario>, IUsuarioEFRepositorio
{
    public UsuarioEF(EFContexto contexto) : base(contexto)
    {
    }
}