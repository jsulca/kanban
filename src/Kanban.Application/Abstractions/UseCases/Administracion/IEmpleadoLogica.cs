using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IEmpleadoLogica
{
    /// <summary>Catálogos de cargos y áreas para el formulario de empleado.</summary>
    ParametrosEmpleado ObtenerParametros();

    List<Empleado> Listar(EmpleadoFiltro? filtro = null);

    /// <summary>Lista vía Entity Framework en lugar de ADO.NET.</summary>
    List<Empleado> ListarEF(EmpleadoFiltro? filtro = null);

    Empleado? Buscar(int id);

    /// <summary>Guarda vía Entity Framework en lugar de ADO.NET.</summary>
    void GuardarEF(Empleado entidad);

    void Guardar(List<Empleado> entidades);

    bool Guardar(Empleado entidad);

    bool Actualizar(Empleado entidad);
}