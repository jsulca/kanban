using System.Text;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Compromisos;

public class AlertaRepositorio : BaseRepositorio, IAlertaRepositorio
{
    public List<Alerta> Listar(int page, int pageSize, int empleadoId)
    {
        var _parametros = new List<NpgsqlParameter>();
        List<Alerta> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.compromisoid, T1.empleadoid, T1.fecharegistro, T1.visto, T2.codigo, T2.descripcion ");
            _query.Append("FROM Alerta T1 ");
            _query.Append("INNER JOIN Compromiso T2 ON T1.compromisoid = T2.id ");
            _query.Append("WHERE T1.empleadoid = @empleadoid ");
            _query.Append("ORDER BY T1.fecharegistro DESC ");

            _query.Append("LIMIT @desde ");
            _query.Append("OFFSET @hasta ");

            _parametros.Add(new NpgsqlParameter("empleadoid", empleadoId));
            _parametros.Add(new NpgsqlParameter("desde", pageSize));
            _parametros.Add(new NpgsqlParameter("hasta", pageSize * (page - 1)));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Alerta
                        {
                            Id = rd.GetFieldValue<int>(0),
                            CompromisoId = rd.GetFieldValue<int>(1),
                            EmpleadoId = rd.GetFieldValue<int>(2),
                            FechaRegistro = rd.GetFieldValue<DateTime>(3),
                            Visto = rd.GetFieldValue<bool>(4),
                            Compromiso = new Domain.Genericos.Compromisos.Compromiso
                            {
                                Codigo = rd.GetFieldValue<string>(5),
                                Descripcion = rd.GetFieldValue<string>(6)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar las alertas.", ex);
        }
    }

    public async Task<List<Alerta>> ListarAsync(int page, int pageSize, int empleadoId)
    {
        var _parametros = new List<NpgsqlParameter>();
        List<Alerta> lista = new();
        ;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.compromisoid, T1.empleadoid, T1.fecharegistro, T1.visto, T2.codigo, T2.descripcion ");
            _query.Append("FROM Alerta T1 ");
            _query.Append("INNER JOIN Compromiso T2 ON T1.compromisoid = T2.id ");
            _query.Append("WHERE T1.empleadoid = @empleadoid ");
            _query.Append("ORDER BY T1.fecharegistro DESC ");

            _query.Append("LIMIT @desde ");
            _query.Append("OFFSET @hasta ");

            _parametros.Add(new NpgsqlParameter("empleadoid", empleadoId));
            _parametros.Add(new NpgsqlParameter("desde", pageSize));
            _parametros.Add(new NpgsqlParameter("hasta", pageSize * (page - 1)));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                        lista.Add(new Alerta
                        {
                            Id = rd.GetFieldValue<int>(0),
                            CompromisoId = rd.GetFieldValue<int>(1),
                            EmpleadoId = rd.GetFieldValue<int>(2),
                            FechaRegistro = rd.GetFieldValue<DateTime>(3),
                            Visto = rd.GetFieldValue<bool>(4),
                            Compromiso = new Domain.Genericos.Compromisos.Compromiso
                            {
                                Codigo = rd.GetFieldValue<string>(5),
                                Descripcion = rd.GetFieldValue<string>(6)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar las alertas.", ex);
        }
    }

    public int Pendientes(int empleadoId)
    {
        var _parametros = new List<NpgsqlParameter>();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT COUNT(1) FROM alerta ");
            _query.Append("WHERE empleadoid = @empleadoid ");
            _query.Append("AND visto = FALSE ");
            _parametros.Add(new NpgsqlParameter("empleadoid", empleadoId));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al contar las alertas pendientes", ex);
        }
    }

    #region Constructores

    public AlertaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion

    #region Transacciones

    public bool Guardar(Alerta entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Alerta_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.EmpleadoId);
                cmd.Parameters.AddWithValue("p1", entidad.CompromisoId);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());

                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de guardar la alerta.", ex);
        }
    }

    public async Task Confirmar(int empleadoId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Alerta_Visto(@p0)";
                cmd.Parameters.AddWithValue("p0", empleadoId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de actualizar el compromiso.", ex);
        }
    }

    #endregion
}