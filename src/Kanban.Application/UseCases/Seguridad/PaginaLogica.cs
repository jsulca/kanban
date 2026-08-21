using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class PaginaLogica(
    IPaginaRepositorio paginas,
    IControlRepositorio controles,
    ITransacciones transacciones)
    : IPaginaLogica
{
    public PagedResult<Pagina> ListarPorPagina(PaginaFiltro filter, int pageIndex, int pageSize)
    {
        return paginas.ListarPorPagina(filter, pageIndex, pageSize);
    }

    public List<Pagina> Listar(bool conDetalles = false)
    {
        var lista = paginas.Listar() ?? [];

        if (conDetalles)
            foreach (var item in lista)
                item.Controles = controles.ListarPorPagina(item.Id) ?? [];

        return lista;
    }

    public Pagina? BuscarPorId(int id, bool conDetalles = false)
    {
        var entidad = paginas.Buscar(id);

        if (entidad is not null && conDetalles)
            entidad.Controles = controles.ListarPorPagina(id) ?? [];

        return entidad;
    }

    public void Guardar(Pagina entidad)
    {
        transacciones.Ejecutar(() =>
        {
            paginas.Guardar(entidad);

            if (entidad.Controles is { Count: > 0 })
                foreach (var item in entidad.Controles)
                {
                    item.PaginaId = entidad.Id;
                    controles.Guardar(item);
                }
        });
    }

    public void Actualizar(Pagina entidad)
    {
        transacciones.Ejecutar(() =>
        {
            paginas.Actualizar(entidad);

            if (entidad.Controles is { Count: > 0 })
                foreach (var item in entidad.Controles)
                    if (item.Id > 0 && item.Eliminado)
                    {
                        controles.Eliminar(item.Id);
                    }
                    else if (item.Id > 0)
                    {
                        controles.Actualizar(item);
                    }
                    else
                    {
                        item.PaginaId = entidad.Id;
                        controles.Guardar(item);
                    }
        });
    }
}