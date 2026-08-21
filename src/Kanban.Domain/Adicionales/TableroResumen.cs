namespace Kanban.Domain.Adicionales;

public class TableroResumen
{
    public int EstructuraId { get; set; }
    public string? Nombre { get; set; }
    public int Nuevo { get; set; }
    public int Pendiente { get; set; }
    public int FueraFecha { get; set; }
    public int PorVerificar { get; set; }
}