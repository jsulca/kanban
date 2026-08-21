namespace Kanban.Domain.Genericos.Verificacion;

public class PlanAccion
{
    public int Id { get; set; }
    public int VerificarId { get; set; }
    public string? Descripcion { get; set; }

    public Verificar? Verificar { get; set; }
    public Compromiso.Compromiso? Compromiso { get; set; }
}