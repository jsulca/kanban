namespace Kanban.Domain.Genericos.Administracion;

public class Sostenibilidad
{
    public int EstructuraId { get; set; }
    public int EmpleadoId { get; set; }

    public Estructura? Estructura { get; set; }
    public Empleado? Empleado { get; set; }
}