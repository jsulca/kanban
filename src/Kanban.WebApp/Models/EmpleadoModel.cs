using Kanban.Domain.Genericos.Administracion;

namespace Kanban.WebApp.Models;

public struct EmpleadoModel
{
    public class Masivo
    {
        public List<Empleado> Empleados { get; set; } = [];
    }
}
