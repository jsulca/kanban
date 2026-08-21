using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface ICargoRepositorio
{
    List<Cargo> Listar();

    Cargo? Buscar(int id);

    bool Guardar(Cargo entidad);

    bool Actualizar(Cargo entidad);
}