using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface IConfirmadorComentarioRepositorio
{
    List<ConfirmadorComentario> Listar(ConfirmadorComentarioFiltro? filtro);

    void Guardar(List<ConfirmadorComentario> entidades);
}