namespace Kanban.WebApp.Models;

public struct EstructuraModel
{
    public class Nuevo
    {
        public int? PadreId { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool Tablero { get; set; }

        public List<int> Areas { get; set; } = [];
    }

    public class Editar
    {
        public int Id { get; set; }
        public int? PadreId { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool Tablero { get; set; }

        public int[] Areas { get; set; } = [];
    }
}
