namespace Kanban.Domain.Genericos.Verificaciones;

public class ConfirmadorComentario
{
    public int EstructuraId { get; set; }
    public int EmpleadoId { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public string? Comentario { get; set; }
}