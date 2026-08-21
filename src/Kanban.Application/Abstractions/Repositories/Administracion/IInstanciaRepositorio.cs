using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IInstanciaRepositorio
{
    List<Instancia> Listar();

    Instancia? Buscar(int id);

    bool Guardar(Instancia entidad);

    bool Actualizar(Instancia entidad);
}