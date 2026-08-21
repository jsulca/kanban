using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class CargoLogica(
    ICargoRepositorio cargos,
    ICargoEFRepositorio cargosEf,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : ICargoLogica
{
    public List<Cargo> Listar()
    {
        return cargos.Listar();
    }

    public Cargo? Buscar(int id)
    {
        return cargos.Buscar(id);
    }

    public bool Guardar(Cargo entidad)
    {
        return transacciones.Ejecutar(() => cargos.Guardar(entidad));
    }

    public void GuardarEF(Cargo entidad)
    {
        cargosEf.Save(entidad);
        unitOfWork.SaveChanges();
    }

    public bool Actualizar(Cargo entidad)
    {
        return transacciones.Ejecutar(() => cargos.Actualizar(entidad));
    }
}