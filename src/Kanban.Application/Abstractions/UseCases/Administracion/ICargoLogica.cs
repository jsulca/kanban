using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface ICargoLogica
{
    List<Cargo> Listar();

    Cargo? Buscar(int id);

    bool Guardar(Cargo entidad);

    /// <summary>Guarda vía Entity Framework en lugar de ADO.NET.</summary>
    void GuardarEF(Cargo entidad);

    bool Actualizar(Cargo entidad);
}