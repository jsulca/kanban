using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class SostenibilidadLogica(ISostenibilidadRepositorio sostenibilidades) : ISostenibilidadLogica
{
    public List<Sostenibilidad> Listar(int estructuraId)
    {
        return sostenibilidades.Listar(estructuraId);
    }
}