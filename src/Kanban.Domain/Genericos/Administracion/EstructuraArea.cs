namespace Kanban.Domain.Genericos.Administracion;

public class EstructuraArea
{
    public int EstructuraId { get; set; }
    public int AreaId { get; set; }

    public Area? Area { get; set; }
}