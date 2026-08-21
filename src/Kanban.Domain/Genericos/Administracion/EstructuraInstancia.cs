namespace Kanban.Domain.Genericos.Administracion;

public class EstructuraInstancia
{
    public int EstructuraId { get; set; }
    public int InstanciaId { get; set; }

    public Estructura? Estructura { get; set; }
    public Instancia? Instancia { get; set; }
}