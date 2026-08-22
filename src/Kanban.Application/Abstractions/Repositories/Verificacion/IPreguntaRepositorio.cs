using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IPreguntaRepositorio
{
    List<Pregunta> Listar(int verificacionId);

    bool Guardar(Pregunta entidad);

    bool Actualizar(Pregunta entidad);
}