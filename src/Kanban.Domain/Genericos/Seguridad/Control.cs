namespace Kanban.Domain.Genericos.Seguridad;

public class Control
{
    public int Id { get; set; }
    public int PaginaId { get; set; }
    public string? Nombre { get; set; }
    public bool Eliminado { get; set; }
}