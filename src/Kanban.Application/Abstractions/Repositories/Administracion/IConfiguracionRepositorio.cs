using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IConfiguracionRepositorio
{
    List<Configuracion> Listar();

    Configuracion? Buscar(string llave);

    void Actualizar(Configuracion entidad);

    void ActualizarVencimiento();
}