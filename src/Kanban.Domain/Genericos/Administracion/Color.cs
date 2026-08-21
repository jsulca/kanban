namespace Kanban.Domain.Genericos.Administracion;

public class Color
{
    public int Id { get; set; }
    public string? Descripcion { get; set; }
    public string? Hex { get; set; }
    public string? Rgba { get; set; }
    public string? Clase { get; set; }

    public List<Area> FondosArea { get; set; } = [];
    public List<Area> TextoArea { get; set; } = [];
}