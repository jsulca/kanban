using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IConfirmadorSemanaRepositorio
{
    List<ConfirmadorSemana> Listar(ConfirmadorSemanaFiltro? filtro);

    void Guardar(List<ConfirmadorSemana> entidades);
}