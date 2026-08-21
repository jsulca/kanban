using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class RolLogica(
    IRolRepositorio roles,
    IRolControlRepositorio controles,
    IRolPaginaRepositorio paginas,
    IRolMenuRepositorio menus,
    ITransacciones transacciones)
    : IRolLogica
{
    public PagedResult<Rol> ListarPorPagina(RolFiltro filter, int pageIndex, int pageSize)
    {
        return roles.ListarPorPagina(filter, pageIndex, pageSize);
    }

    public List<Rol> Listar()
    {
        return roles.Listar() ?? [];
    }

    public Rol? BuscarPorId(int id, bool conDetalles = false)
    {
        var entidad = roles.BuscarPorId(id);

        if (entidad is not null && conDetalles)
        {
            entidad.Controles = controles.Listar(id) ?? [];
            entidad.Paginas = paginas.Listar(id) ?? [];
            entidad.Menus = menus.Listar(id) ?? [];
        }

        return entidad;
    }

    public void Guardar(Rol entidad)
    {
        transacciones.Ejecutar(() =>
        {
            roles.Guardar(entidad);

            if (entidad.Controles is { Count: > 0 })
                foreach (var item in entidad.Controles)
                {
                    item.RolId = entidad.Id;
                    controles.Guardar(item);
                }

            if (entidad.Paginas is { Count: > 0 })
                foreach (var item in entidad.Paginas)
                {
                    item.RolId = entidad.Id;
                    paginas.Guardar(item);
                }

            if (entidad.Menus is { Count: > 0 })
                foreach (var item in entidad.Menus)
                {
                    item.RolId = entidad.Id;
                    menus.Guardar(item);
                }
        });
    }

    public void Actualizar(Rol entidad)
    {
        transacciones.Ejecutar(() =>
        {
            roles.Actualizar(entidad);

            controles.Limpiar(entidad.Id);
            if (entidad.Controles is { Count: > 0 })
                foreach (var control in entidad.Controles)
                {
                    control.RolId = entidad.Id;
                    controles.Guardar(control);
                }

            paginas.Limpiar(entidad.Id);
            if (entidad.Paginas is { Count: > 0 })
                foreach (var pagina in entidad.Paginas)
                {
                    pagina.RolId = entidad.Id;
                    paginas.Guardar(pagina);
                }

            menus.Limpiar(entidad.Id);
            if (entidad.Menus is { Count: > 0 })
                foreach (var menu in entidad.Menus)
                {
                    menu.RolId = entidad.Id;
                    menus.Guardar(menu);
                }
        });
    }
}