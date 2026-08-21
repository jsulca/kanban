using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IUsuarioEstructuraRepositorio
{
    List<UsuarioEstructura>? Listar(int usuarioId);

    void Guardar(List<UsuarioEstructura> estructuras);

    void Limpiar(int usuarioId);
}