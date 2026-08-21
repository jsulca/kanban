namespace Kanban.Domain.Genericos.Seguridad;

public class Menu
{
    public int Id { get; set; }
    public int? PadreId { get; set; }
    public string? Nombre { get; set; }
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    public TipoMenu Tipo { get; set; }
}