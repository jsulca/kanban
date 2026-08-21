using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IInstanciaLogica
{
    List<Instancia> Listar();

    Instancia? Buscar(int id);

    bool Guardar(Instancia entidad);

    bool Actualizar(Instancia entidad);
}