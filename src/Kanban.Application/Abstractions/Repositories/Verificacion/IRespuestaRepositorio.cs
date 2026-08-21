using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IRespuestaRepositorio
{
    List<Respuesta> Listar(int verificacionId);

    bool Guardar(Respuesta entidad);

    bool Limpiar(int preguntaId);
}