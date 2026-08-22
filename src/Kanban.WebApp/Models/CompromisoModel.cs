using Kanban.Domain;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.WebApp.Models;

public struct CompromisoModel
{
    #region MVC

    public class Nuevo
    {
        public string? Descripcion { get; set; }
        public int? TableroId { get; set; }
        public string? Detalle { get; set; }
        public string? Impacto { get; set; }
        public string? Origen { get; set; }
        public int? PlanAccionId { get; set; }
        public IFormFile? Foto { get; set; }

        public Compromiso Get()
        {
            return new Compromiso
            {
                Descripcion = Descripcion,
                TableroId = TableroId.Value,
                Detalle = Detalle,
                Impacto = Impacto,
                Origen = Origen,
                PlanAccionId = PlanAccionId
            };
        }
    }

    public class Editar
    {
        public int? Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Detalle { get; set; }
        public string? Impacto { get; set; }

        public Compromiso Get()
        {
            return new Compromiso
            {
                Id = Id.Value,
                Descripcion = Descripcion,
                Detalle = Detalle,
                Impacto = Impacto
            };
        }
    }

    public class Verificar
    {
        public int? Id { get; set; }
        public string? Respuesta { get; set; }
        public DateTime? Fecha { get; set; }
    }

    public class Asignar
    {
        public int Id { get; set; }
        public int? ResponsableId { get; set; }
        public int? AreaId { get; set; }
        public bool? PorVerificar { get; set; }
        public string? Accion { get; set; }
        public bool? Finalizo { get; set; }
    }

    public class CambiarEstado
    {
        public int Id { get; set; }
        public EstadoCompromiso Estado { get; set; }
        public DateTime? FechaProgramacion { get; set; }
        public DateTime? FechaReprogramacion { get; set; }
        public string? Respuesta { get; set; }
        public string? Motivo { get; set; }
        public string? Accion { get; set; }
    }

    public class Rechazar
    {
        public int? Id { get; set; }
        public string? Motivo { get; set; }
    }

    public class PorVerificar
    {
        public int? Id { get; set; }
    }

    public class Finalizar
    {
        public int? Id { get; set; }
    }

    public class EscalarGerencia
    {
        public int? Id { get; set; }
        public string? Motivo { get; set; }
    }

    public class Indicador
    {
        public int TableroId { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }

        public DateTime FechaDesde { get => new DateTime(Anio, Mes, 1); }
        public DateTime FechaHasta { get => new DateTime(Anio, Mes, DateTime.DaysInMonth(Anio, Mes)); }
    }

    public class Reprogramar
    {
        public int Id { get; set; }
        public DateTime? FechaReprogramacion { get; set; }
    }

    public class ExportarExcel
    {
        public int TableroId { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }

    #endregion

    #region API

    public class Listar
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int[] EstructurasId { get; set; } = [];
    }

    public class Post
    {
        public int? UsuarioId { get; set; }
        public int? EmpleadoId { get; set; }
        public int? EstructuraId { get; set; }
        public string? Descripcion { get; set; }
        public int? TableroId { get; set; }
        public string? Detalle { get; set; }
        public string? Impacto { get; set; }
        public string? Origen { get; set; }
        public IFormFile? Foto { get; set; }

        public Compromiso Get()
        {
            return new Compromiso
            {
                Descripcion = Descripcion,
                TableroId = TableroId.Value,
                Detalle = Detalle,
                Impacto = Impacto,
                Origen = Origen,
                UsuarioRegistroId = UsuarioId.Value,
                EmpleadoRegistroId = EmpleadoId.Value,
                EstructuraId = EstructuraId.Value
            };
        }
    }

    public class Buscar
    {
        public int? Id { get; set; }
    }

    public class Asignados
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int EmpleadoId { get; set; }
    }

    public class VerificarAPI
    {
        public int EmpleadoId { get; set; }
        public int UsuarioId { get; set; }
        public int Id { get; set; }
    }

    public class Resumen
    {
        public List<UsuarioEstructura> Tableros { get; set; } = [];
    }

    #endregion
}
