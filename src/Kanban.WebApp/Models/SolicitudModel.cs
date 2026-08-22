using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.WebApp.Models;

public struct SolicitudModel
{
    #region API

    public class Guardar
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? NroDocumento { get; set; }

        public Solicitud Get() => new Solicitud
        {
            Nombre = Nombre,
            Apellido = Apellido,
            Correo = Correo,
            Telefono = Telefono,
            NroDocumento = NroDocumento
        };
    }

    #endregion
}
