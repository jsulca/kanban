namespace Kanban.Domain.Genericos.Seguridad;

public class RolPagina
{
    public int RolId { get; set; }
    public int PaginaId { get; set; }

    public Pagina? Pagina { get; set; }
}