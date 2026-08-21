using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IAdjuntoRepositorio
{
    Adjunto? Buscar(int id);

    bool Guardar(Adjunto entidad);
}