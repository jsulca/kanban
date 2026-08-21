using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class EstructuraAreaLogica(IEstructuraAreaRepositorio areas) : IEstructuraAreaLogica
{
    public List<EstructuraArea> Listar(int estructuraId)
    {
        return areas.Listar(estructuraId);
    }
}