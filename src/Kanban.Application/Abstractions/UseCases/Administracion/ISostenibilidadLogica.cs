using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface ISostenibilidadLogica
{
    List<Sostenibilidad> Listar(int estructuraId);
}