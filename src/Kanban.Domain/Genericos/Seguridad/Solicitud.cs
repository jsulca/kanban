namespace Kanban.Domain.Genericos.Seguridad;

public class Solicitud
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? NroDocumento { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
}