using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class IndicadorLogica(IIndicadorRepositorio indicadores, ITransacciones transacciones)
    : IIndicadorLogica
{
    public List<Indicador> Listar(IndicadorFiltro? filtro = null)
    {
        return indicadores.Listar(filtro);
    }

    public Indicador? Buscar(int id)
    {
        return indicadores.Buscar(id);
    }

    public bool Guardar(Indicador entidad)
    {
        return transacciones.Ejecutar(() => indicadores.Guardar(entidad));
    }

    public bool Actualizar(Indicador entidad)
    {
        return transacciones.Ejecutar(() => indicadores.Actualizar(entidad));
    }
}