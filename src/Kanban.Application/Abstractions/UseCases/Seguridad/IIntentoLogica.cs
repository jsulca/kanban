using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.Abstractions.UseCases.Seguridad;

public interface IIntentoLogica
{
    List<Intento> Listar(string usuario, int pageSize);

    void Guardar(Intento entidad);
}