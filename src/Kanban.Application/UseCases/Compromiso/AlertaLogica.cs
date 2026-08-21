using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.Domain.Genericos.Compromiso;

namespace Kanban.Application.UseCases.Compromiso;

public class AlertaLogica(IAlertaRepositorio alertas, ITransacciones transacciones) : IAlertaLogica
{
    public List<Alerta> Listar(int page, int pageSize, int empleadoId)
    {
        return alertas.Listar(page, pageSize, empleadoId);
    }

    public Task<List<Alerta>> ListarAsync(int page, int pageSize, int empleadoId)
    {
        return alertas.ListarAsync(page, pageSize, empleadoId);
    }

    public int Pendientes(int empleadoId)
    {
        return alertas.Pendientes(empleadoId);
    }

    public Task ConfirmarAlertasAsync(int empleadoId)
    {
        return transacciones.EjecutarAsync(() => alertas.Confirmar(empleadoId));
    }
}