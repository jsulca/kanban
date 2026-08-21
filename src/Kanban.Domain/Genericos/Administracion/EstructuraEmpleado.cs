namespace Kanban.Domain.Genericos.Administracion;

public class EstructuraEmpleado
{
    public int EstructuraId { get; set; }
    public int EmpleadoId { get; set; }
    public int AreaId { get; set; }

    public Estructura? Estructura { get; set; }
    public Empleado? Empleado { get; set; }
    public Area? Area { get; set; }
}