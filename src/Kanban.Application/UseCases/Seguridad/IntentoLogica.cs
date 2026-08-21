using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class IntentoLogica(
    IIntentoRepositorio intentos,
    IIntentoEFRepositorio intentosEf,
    IUnitOfWork unitOfWork)
    : IIntentoLogica
{
    public List<Intento> Listar(string usuario, int pageSize)
    {
        return intentos.Listar(usuario, pageSize);
    }

    public void Guardar(Intento entidad)
    {
        // guarda por EF; la variante ADO estaba comentada en .NET Framework
        intentosEf.Save(entidad);
        unitOfWork.SaveChanges();
    }
}