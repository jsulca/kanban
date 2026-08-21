using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IEstructuraAreaRepositorio
{
    List<EstructuraArea> Listar(int estructuraId);

    void Guardar(EstructuraArea entidad);

    void Guardar(List<EstructuraArea> entidades);

    bool Limpiar(int estructuraId);
}