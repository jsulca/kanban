using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IEstructuraInstanciaRepositorio
{
    List<EstructuraInstancia> Listar(int estructuraId);

    List<EstructuraInstancia> ListarInstancia(int estructuraId);

    void Guardar(EstructuraInstancia entidad);

    void Guardar(List<EstructuraInstancia> entidades);

    bool Limpiar(int estructuraId);
}