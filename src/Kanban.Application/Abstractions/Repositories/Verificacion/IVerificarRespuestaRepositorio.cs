using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IVerificarRespuestaRepositorio
{
    List<VerificarRespuesta>? Listar(int verificarId);

    bool Guardar(VerificarRespuesta entidad);
}