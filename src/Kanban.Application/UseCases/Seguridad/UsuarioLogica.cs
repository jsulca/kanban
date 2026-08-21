using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class UsuarioLogica(
    IUsuarioRepositorio usuarios,
    IUsuarioEFRepositorio usuariosEf,
    IUsuarioEstructuraRepositorio estructuras,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : IUsuarioLogica
{
    public PagedResult<Usuario> ListarPorPagina(UsuarioFiltro filter, int pageIndex, int pageSize)
    {
        return usuarios.ListarPorPagina(filter, pageIndex, pageSize);
    }

    public Usuario? BuscarPorId(int id, bool conDetalles = false)
    {
        var entidad = usuarios.Buscar(id);

        if (entidad is not null && conDetalles)
            entidad.Estructuras = estructuras.Listar(id) ?? [];

        return entidad;
    }

    public List<UsuarioEstructura> BuscarPorUsuario(int usuarioId)
    {
        return estructuras.Listar(usuarioId) ?? [];
    }

    public Usuario? BuscarLogin(string nombre, string clave)
    {
        var entidad = usuarios.BuscarLogin(nombre, clave);

        if (entidad is not null)
            entidad.Estructuras = estructuras.Listar(entidad.Id) ?? [];

        return entidad;
    }

    public void Guardar(Usuario entidad)
    {
        transacciones.Ejecutar(() =>
        {
            usuarios.Guardar(entidad);

            if (entidad.Id > 0 && entidad.Estructuras != null)
            {
                entidad.Estructuras.ForEach(x => x.UsuarioId = entidad.Id);
                estructuras.Guardar(entidad.Estructuras);
            }
        });
    }

    public void GuardarEF(Usuario entidad)
    {
        usuariosEf.Save(entidad);
        unitOfWork.SaveChanges();
    }

    public void Actualizar(Usuario entidad)
    {
        transacciones.Ejecutar(() =>
        {
            usuarios.Actualizar(entidad);
            estructuras.Limpiar(entidad.Id);

            if (entidad.Id > 0 && entidad.Estructuras != null)
            {
                entidad.Estructuras.ForEach(x => x.UsuarioId = entidad.Id);
                estructuras.Guardar(entidad.Estructuras);
            }

            if (!string.IsNullOrEmpty(entidad.Clave))
                usuarios.CambiarClave(new Usuario { Id = entidad.Id, Clave = entidad.Clave });
        });
    }

    public void Desactivar(int usuarioId)
    {
        transacciones.Ejecutar(() =>
        {
            var usuario = usuarios.Buscar(usuarioId)
                          ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");

            usuario.Activo = false;
            usuarios.Actualizar(usuario);
        });
    }

    public void CambiarClave(Usuario model)
    {
        transacciones.Ejecutar(() => usuarios.CambiarClave(model));
    }

    public void Token(int id, string token, string ip)
    {
        transacciones.Ejecutar(() => usuarios.Token(id, token, ip));
    }

    public bool ValidarToken(int id, string token, string ip)
    {
        return usuarios.ValidarToken(id, token, ip);
    }

    public bool ExisteUsuario(int id, string usuario)
    {
        return usuarios.ContarUsuario(id, usuario) > 0;
    }
}