using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface ICategoriaRepositorio
{
    List<Categoria> Listar(int verificacionId);

    void Guardar(Categoria entidad);

    void Actualizar(Categoria entidad);
}