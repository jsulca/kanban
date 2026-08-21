using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IIntentoRepositorio
{
    List<Intento> Listar(string usuario, int pageSize);

    bool Guardar(Intento entidad);
}