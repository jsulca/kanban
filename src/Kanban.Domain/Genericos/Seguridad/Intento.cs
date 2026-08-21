namespace Kanban.Domain.Genericos.Seguridad;

public class Intento
{
    public int Id { get; set; }
    public string? Usuario { get; set; }
    public string? Clave { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaRegistro { get; set; }
}