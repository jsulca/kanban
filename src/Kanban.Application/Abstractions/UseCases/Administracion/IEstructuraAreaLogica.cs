using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IEstructuraAreaLogica
{
    List<EstructuraArea> Listar(int estructuraId);
}