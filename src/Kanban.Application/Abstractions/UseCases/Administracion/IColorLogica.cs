using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IColorLogica
{
    List<Color> Listar();

    Color? Buscar(int id);

    bool Guardar(Color entidad);

    bool Actualizar(Color entidad);
}