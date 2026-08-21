using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.Repositories.Seguridad;

public interface IUsuarioRepositorio
{
    int ContarUsuario(int id, string usuario);

    PagedResult<Usuario> ListarPorPagina(UsuarioFiltro? filter, int page, int pageSize);

    Usuario? BuscarLogin(string nombre, string clave);

    Usuario? Buscar(int id);

    void Guardar(Usuario entidad);

    void Actualizar(Usuario entidad);

    void CambiarClave(Usuario entidad);

    void Token(int id, string token, string ip);

    bool ValidarToken(int id, string token, string ip);
}