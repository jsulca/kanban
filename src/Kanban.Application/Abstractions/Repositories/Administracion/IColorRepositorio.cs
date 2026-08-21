using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IColorRepositorio
{
    List<Color> Listar();

    Color? Buscar(int id);

    bool Guardar(Color entidad);

    bool Actualizar(Color entidad);
}