namespace Kanban.Domain.Genericos.Administracion;

public class TipoVerificacion
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public bool Activo { get; set; }
    public bool VP { get; set; }
    public bool IGP { get; set; }
}