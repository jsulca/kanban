using Kanban.Domain.Genericos.Compromiso;

namespace Kanban.Application.Abstractions.Repositories.Compromiso;

public interface IAlertaRepositorio
{
    List<Alerta> Listar(int page, int pageSize, int empleadoId);

    Task<List<Alerta>> ListarAsync(int page, int pageSize, int empleadoId);

    int Pendientes(int empleadoId);

    bool Guardar(Alerta entidad);

    Task Confirmar(int empleadoId);
}