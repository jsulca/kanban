using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class AdjuntoLogica(IAdjuntoRepositorio adjuntos) : IAdjuntoLogica
{
    public Adjunto? Buscar(int id)
    {
        return adjuntos.Buscar(id);
    }
}