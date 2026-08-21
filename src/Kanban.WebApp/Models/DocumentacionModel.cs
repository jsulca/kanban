namespace Kanban.WebApp.Models;

/// <summary>
///     Un tema del manual. Las secciones son listas de estos, y la vista parcial
///     <c>_Tema.cshtml</c> los pinta siempre con los mismos bloques para que el manual
///     se lea igual en todas partes.
/// </summary>
public struct DocumentacionModel
{
    public class Tema
    {
        /// <summary>Ancla de la página, usada por el índice lateral.</summary>
        public string Id { get; set; } = "";

        public string Titulo { get; set; } = "";

        public string Icono { get; set; } = "far fa-file-alt";

        /// <summary>Dónde está la pantalla, normalmente su dirección.</summary>
        public string Ruta { get; set; } = "";

        public string ParaQue { get; set; } = "";

        public List<string> Pasos { get; set; } = [];

        public List<string> Reglas { get; set; } = [];

        /// <summary>Mensajes que puede ver el usuario y qué significan.</summary>
        public List<Mensaje> Errores { get; set; } = [];

        /// <summary>Nombre del archivo dentro de <c>wwwroot/images/docs</c>.</summary>
        public string? Captura { get; set; }

        public Tema()
        {
        }
    }

    public class Mensaje(string texto, string significado)
    {
        public string Texto { get; set; } = texto;

        public string Significado { get; set; } = significado;
    }
}
