using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface IUsuarioLogica
{
    PagedResult<Usuario> ListarPorPagina(UsuarioFiltro filter, int pageIndex, int pageSize);

    Usuario? BuscarPorId(int id, bool conDetalles = false);

    List<UsuarioEstructura> BuscarPorUsuario(int usuarioId);

    Usuario? BuscarLogin(string nombre, string clave);

    void Guardar(Usuario entidad);

    /// <summary>Guarda vía Entity Framework en lugar de ADO.NET.</summary>
    void GuardarEF(Usuario entidad);

    void Actualizar(Usuario entidad);

    void Desactivar(int usuarioId);

    void CambiarClave(Usuario model);

    void Token(int id, string token, string ip);

    bool ValidarToken(int id, string token, string ip);

    bool ExisteUsuario(int id, string usuario);
}