using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class EstructuraInstanciaLogica(IEstructuraInstanciaRepositorio instancias) : IEstructuraInstanciaLogica
{
    public List<EstructuraInstancia> Listar(int estructuraId)
    {
        return instancias.Listar(estructuraId);
    }
}