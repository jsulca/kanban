using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Base;
using Kanban.Infrastructure.Context;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class VerificarRespuestaEF : RepositorioGenerico<VerificarRespuesta>, IVerificarRespuestaEFRepositorio
{
    public VerificarRespuestaEF(EFContexto contexto) : base(contexto)
    {
    }
}