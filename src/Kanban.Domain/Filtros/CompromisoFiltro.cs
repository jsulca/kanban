namespace Kanban.Domain.Filtros;

public class CompromisoFiltro
{
    public int? TableroId { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public string? FechaRegistroDesde { get; set; }
    public string? FechaRegistroHasta { get; set; }
    public EstadoCompromiso? Estado { get; set; }
    public int? InstanciaId { get; set; }
    public string? EstructuraDescripcion { get; set; }
    public int? ResponsableId { get; set; }

    public int[]? Estados { get; set; }
    public int[]? Instancias { get; set; }
    public int[]? Estructuras { get; set; }
}