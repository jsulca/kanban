using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Domain.Genericos.Seguridad;

public class UsuarioEstructura
{
    public int UsuarioId { get; set; }
    public int EstructuraId { get; set; }
    public bool Acceso { get; set; }

    public Usuario? Usuario { get; set; }
    public Estructura? Estructura { get; set; }
}