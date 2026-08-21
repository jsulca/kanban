using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IAdjuntoLogica
{
    Adjunto? Buscar(int id);
}