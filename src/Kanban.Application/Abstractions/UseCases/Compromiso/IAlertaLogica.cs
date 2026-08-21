using Kanban.Domain.Genericos.Compromiso;

namespace Kanban.Application.Abstractions.UseCases.Compromiso;

public interface IAlertaLogica
{
    List<Alerta> Listar(int page, int pageSize, int empleadoId);

    Task<List<Alerta>> ListarAsync(int page, int pageSize, int empleadoId);

    int Pendientes(int empleadoId);

    Task ConfirmarAlertasAsync(int empleadoId);
}