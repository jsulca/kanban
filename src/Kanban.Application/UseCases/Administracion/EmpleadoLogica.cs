using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class EmpleadoLogica(
    IEmpleadoRepositorio empleados,
    IEmpleadoEFRepositorio empleadosEf,
    ICargoRepositorio cargos,
    IAreaRepositorio areas,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : IEmpleadoLogica
{
    public ParametrosEmpleado ObtenerParametros()
    {
        return new ParametrosEmpleado(cargos.Listar(), areas.Listar());
    }

    public List<Empleado> Listar(EmpleadoFiltro? filtro = null)
    {
        return empleados.Listar(filtro);
    }

    public List<Empleado> ListarEF(EmpleadoFiltro? filtro = null)
    {
        // el filtro se ignora, igual que en la versión de .NET Framework
        return empleadosEf.GetAll();
    }

    public Empleado? Buscar(int id)
    {
        return empleados.Buscar(id);
    }

    public void GuardarEF(Empleado entidad)
    {
        empleadosEf.Save(entidad);
        unitOfWork.SaveChanges();
    }

    public void Guardar(List<Empleado> entidades)
    {
        transacciones.Ejecutar(() => empleados.Guardar(entidades));
    }

    public bool Guardar(Empleado entidad)
    {
        return transacciones.Ejecutar(() => empleados.Guardar(entidad));
    }

    public bool Actualizar(Empleado entidad)
    {
        return transacciones.Ejecutar(() => empleados.Actualizar(entidad));
    }
}