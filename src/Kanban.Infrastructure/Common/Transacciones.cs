using System.Data;
using Kanban.Application.Abstractions;
using Npgsql;

namespace Kanban.Infrastructure.Common;

/// <summary>
///     Implementa <see cref="ITransacciones" /> sobre la <see cref="NpgsqlConnection" />
///     del ámbito actual, que es la misma que usan todos los repositorios.
/// </summary>
public sealed class Transacciones : ITransacciones
{
    private readonly NpgsqlConnection _cn;

    /// <summary>Transacción en curso, si ya estamos dentro de una.</summary>
    private NpgsqlTransaction? _actual;

    public Transacciones(NpgsqlConnection connection)
    {
        _cn = connection;
    }

    public void Ejecutar(Action operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        Ejecutar<object?>(() =>
        {
            operacion();
            return null;
        });
    }

    public T Ejecutar<T>(Func<T> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        // Postgres no admite transacciones anidadas: si ya hay una abierta en este
        // ámbito, la operación se une a ella y el commit lo hace la llamada externa.
        if (_actual is not null) return operacion();

        if (_cn.State == ConnectionState.Closed) _cn.Open();

        using var trx = _cn.BeginTransaction();
        _actual = trx;
        try
        {
            var resultado = operacion();
            trx.Commit();
            return resultado;
        }
        finally
        {
            // sin Commit, el Dispose del using deshace la transacción
            _actual = null;
        }
    }

    public async Task EjecutarAsync(Func<Task> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        await EjecutarAsync<object?>(async () =>
        {
            await operacion();
            return null;
        });
    }

    public async Task<T> EjecutarAsync<T>(Func<Task<T>> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        if (_actual is not null) return await operacion();

        if (_cn.State == ConnectionState.Closed) await _cn.OpenAsync();

        await using var trx = await _cn.BeginTransactionAsync();
        _actual = trx;
        try
        {
            var resultado = await operacion();
            await trx.CommitAsync();
            return resultado;
        }
        finally
        {
            _actual = null;
        }
    }
}
