namespace Kanban.Domain.Genericos.Seguridad;

public class Rol
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public bool Activo { get; set; }

    public List<RolMenu> Menus { get; set; } = [];
    public List<RolPagina> Paginas { get; set; } = [];
    public List<RolControl> Controles { get; set; } = [];
}