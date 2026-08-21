namespace Kanban.Domain.Genericos.Seguridad;

public class RolControl
{
    public int RolId { get; set; }
    public int ControlId { get; set; }

    public Control? Control { get; set; }
}