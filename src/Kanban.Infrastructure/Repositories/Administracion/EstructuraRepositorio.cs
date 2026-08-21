using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Adicionales;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EstructuraRepositorio : BaseRepositorio, IEstructuraRepositorio
{
    public List<Estructura> Listar(EstructuraFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        List<Estructura> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.padreid, T1.codigo, T1.descripcion, T1.tablero ");
            _query.Append("FROM Estructura T1 ");
            if (filtro != null)
            {
                _query.Append("WHERE 1 = 1 ");
                _query.Append("AND T1.tablero = @tablero");
                _parametros.Add(new NpgsqlParameter("tablero", filtro.Tablero ? 1 : 0));
            }

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Estructura
                        {
                            Id = rd.GetInt32(0),
                            PadreId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            Codigo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Descripcion = rd.GetString(3),
                            Tablero = rd.GetBoolean(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las estructuras.", ex);
        }
    }

    public List<Estructura> Arbol(int id)
    {
        List<Estructura> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("WITH RECURSIVE EstructuraTree (id, padreid, codigo, descripcion, tablero) ");
            _query.Append("AS ");
            _query.Append("( ");
            _query.Append("SELECT T1.id, T1.padreid, T1.codigo, T1.descripcion, T1.tablero ");
            _query.Append("FROM Estructura T1 ");
            _query.Append("WHERE T1.padreid = @padreid ");
            _query.Append("UNION ALL ");
            _query.Append("SELECT T1.id, T1.padreid, T1.codigo, T1.descripcion, T1.tablero ");
            _query.Append("FROM Estructura T1 ");
            _query.Append("INNER JOIN EstructuraTree T2 ON T2.id = T1.padreid ");
            _query.Append(") ");
            _query.Append("SELECT id, padreid, codigo, descripcion, tablero ");
            _query.Append("FROM Estructura ");
            _query.Append("WHERE id = @padreid ");
            _query.Append("UNION ALL ");
            _query.Append("SELECT id, padreid, codigo, descripcion, tablero ");
            _query.Append("FROM EstructuraTree ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("padreid", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Estructura
                        {
                            Id = rd.GetInt32(0),
                            PadreId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            Codigo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Descripcion = rd.GetString(3),
                            Tablero = rd.GetBoolean(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar el arbol las estructuras.", ex);
        }
    }

    public Estructura? Buscar(int id)
    {
        Estructura? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.padreid, T1.codigo, T1.descripcion, T1.tablero ");
            _query.Append("FROM Estructura T1 ");
            _query.Append("WHERE T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Estructura
                        {
                            Id = rd.GetInt32(0),
                            PadreId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            Codigo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Descripcion = rd.GetString(3),
                            Tablero = rd.GetBoolean(4)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar una estructura.", ex);
        }
    }

    public bool Guardar(Estructura entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Estructura_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.PadreId ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.Tablero);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una estructura.", ex);
        }
    }

    public bool Actualizar(Estructura entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Estructura_Actualizar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.Tablero);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una estructura.", ex);
        }
    }

    public bool TieneTablero(int id)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Estructura_TieneTablero(@id)";
                cmd.Parameters.AddWithValue("id", id);

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al validar.", ex);
        }
    }

    public string? Ruta(int id)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Estructura_Ruta(@id)";
                cmd.Parameters.AddWithValue("id", id);

                return cmd.ExecuteScalar()?.ToString();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al crear la ruta.", ex);
        }
    }

    public List<TableroResumen> Resumen(int[] tableros)
    {
        List<TableroResumen> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.descripcion, ");
            _query.Append("SUM(CASE WHEN T2.estado = 1 AND T2.instanciaid IS NULL THEN 1 ELSE 0 END) AS nuevo, ");
            _query.Append("SUM(CASE WHEN T2.estado = 2 AND T2.instanciaid IS NULL THEN 1 ELSE 0 END) AS pendiente, ");
            _query.Append("SUM(CASE WHEN T2.estado = 3 AND T2.instanciaid IS NULL THEN 1 ELSE 0 END) AS fuera_fecha, ");
            _query.Append(
                "SUM(CASE WHEN T2.estado = 6 AND T2.instanciaid IS NULL THEN 1 ELSE 0 END) AS por_verificar ");
            _query.Append("FROM Estructura T1 ");
            _query.Append(
                "LEFT JOIN Compromiso T2 ON T1.id = T2.tableroid AND T2.estado IN (1, 2, 3, 6) AND T2.instanciaid IS NULL ");
            _query.Append("WHERE T1.tablero = TRUE ");
            _query.Append("AND T1.id::TEXT IN (SELECT * FROM regexp_split_to_table(@tableros, ',')) ");
            _query.Append("GROUP BY T1.id, T1.descripcion ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("tableros", string.Join(",", tableros));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new TableroResumen
                        {
                            EstructuraId = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Nuevo = rd.GetInt32(2),
                            Pendiente = rd.GetInt32(3),
                            FueraFecha = rd.GetInt32(4),
                            PorVerificar = rd.GetInt32(5)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar el resumen de los tableros.", ex);
        }
    }

    #region Constructores

    public EstructuraRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}