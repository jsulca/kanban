namespace Kanban.Domain.Genericos.Seguridad;

public class RolMenu
{
    public int RolId { get; set; }
    public int MenuId { get; set; }

    public Menu? Menu { get; set; }
}