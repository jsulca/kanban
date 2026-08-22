using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.WebApp.Models;

public struct VerificacionModel
{
    #region API

    public class Listar
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int EmpleadoId { get; set; }
    }

    public class ListarTipoVerificacion
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class BuscarTipoVerificacion
    {
        public int Id { get; set; }
    }

    public class ListarCelula
    {
        public int Id { get; set; }
    }

    public class Guardar
    {
        public int? UsuarioId { get; set; }
        public int? EmpleadoId { get; set; }
        public int? TableroId { get; set; }
        public int? VerificacionId { get; set; }
        public string? Encargado { get; set; }
        public bool? Rom { get; set; }
        public string? NroRom { get; set; }
        public string? Fortaleza { get; set; }
        public string? Oportunidad { get; set; }
        public string? InstructivoEstandar { get; set; }
        public int? EstructuraId { get; set; }
        /*
         
                cmd.Parameters.AddWithValue("@empleadoid", entidad.EmpleadoId);
                cmd.Parameters.AddWithValue("@verificacionid", entidad.VerificacionId);
                cmd.Parameters.AddWithValue("@encargado", entidad.Encargado ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@rom", entidad.Rom ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@nrorom", entidad.NroRom ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@fortaleza", entidad.Fortaleza ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@oportunidad", entidad.Oportunidad ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@instructivoestandar", entidad.InstructivoEstandar ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@puntajemaximo", entidad.PuntajeMaximo);
                cmd.Parameters.AddWithValue("@puntajeobtenido", entidad.PuntajeObtenido);
                cmd.Parameters.AddWithValue("@tableroid", entidad.TableroId);
                cmd.Parameters.AddWithValue("@usuarioid", entidad.UsuarioId);
                cmd.Parameters.AddWithValue("@vp", entidad.VP);
                cmd.Parameters.AddWithValue("@areaid", entidad.AreaId ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@estructuraid", entidad.EstructuraId);
         
         */

        public List<VerificarRespuesta> Respuestas { get; set; } = [];
        public List<PlanAccion> PlanesAccion { get; set; } = [];

        public Verificar Get()
        {

            return new Verificar()
            {
                UsuarioId = UsuarioId.Value,
                EmpleadoId = EmpleadoId.Value,
                TableroId = TableroId.Value,
                VerificacionId = VerificacionId.Value,
                Encargado = Encargado,
                Rom = Rom,
                NroRom = NroRom,
                Fortaleza = Fortaleza,
                Oportunidad = Oportunidad,
                InstructivoEstandar = InstructivoEstandar,
                EstructuraId = EstructuraId.Value,
                Respuestas = Respuestas,
                PlanesAccion = PlanesAccion
            };
        }
    }

    public class Buscar
    {
        public int? Id { get; set; }
    }

    public class GuardarTablero
    {
        public List<ConfirmadorComentario> Comentarios { get; set; } = [];
        public List<ConfirmadorSemana> Semanas { get; set; } = [];
        public List<SostenibilidadMes> Meses { get; set; } = [];
    }

    public class Reporte
    {
        public int TableroId { get; set; }
        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }
    }

    #endregion
}
