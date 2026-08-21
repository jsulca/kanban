using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IConfiguracionLogica
{
    List<Configuracion> Listar();

    Configuracion? Buscar(string llave);

    void Actualizar(Configuracion entidad);
}