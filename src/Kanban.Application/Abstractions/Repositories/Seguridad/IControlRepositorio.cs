using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IControlRepositorio
{
    /// <summary>
    ///     Lista los controles de una página de la aplicación. No es paginación:
    ///     <paramref name="paginaid" /> es el identificador de la pantalla.
    /// </summary>
    List<Control>? ListarPorPagina(int paginaid);

    void Guardar(Control entidad);

    void Actualizar(Control entidad);

    void Eliminar(int id);
}