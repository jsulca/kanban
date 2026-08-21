using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificacion;

namespace Kanban.Application.Abstractions.Repositories.Verificacion;

public interface ISostenibilidadMesRepositorio
{
    List<SostenibilidadMes> Listar(SostenibilidadMesFiltro? filtro);

    void Guardar(List<SostenibilidadMes> entidades);
}