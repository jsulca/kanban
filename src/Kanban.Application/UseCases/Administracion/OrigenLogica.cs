using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class OrigenLogica(IOrigenRepositorio origenes, ITransacciones transacciones) : IOrigenLogica
{
    public List<Origen> Listar(OrigenFiltro? filtro = null)
    {
        return origenes.Listar(filtro);
    }

    public Origen? Buscar(int id)
    {
        return origenes.Buscar(id);
    }

    public bool Guardar(Origen entidad)
    {
        return transacciones.Ejecutar(() => origenes.Guardar(entidad));
    }

    public bool Actualizar(Origen entidad)
    {
        return transacciones.Ejecutar(() => origenes.Actualizar(entidad));
    }
}