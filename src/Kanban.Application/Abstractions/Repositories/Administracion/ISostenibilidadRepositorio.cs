using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface ISostenibilidadRepositorio
{
    List<Sostenibilidad> Listar(int estructuraId);

    void Guardar(Sostenibilidad entidad);

    void Guardar(List<Sostenibilidad> entidades);

    bool Limpiar(int estructuraId);
}