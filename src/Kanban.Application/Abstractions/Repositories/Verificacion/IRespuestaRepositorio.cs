using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IRespuestaRepositorio
{
    List<Respuesta> Listar(int verificacionId);

    bool Guardar(Respuesta entidad);

    bool Limpiar(int preguntaId);
}