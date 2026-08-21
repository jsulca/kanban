namespace Kanban.Application.Abstractions;

/// <summary>
///     Ejecuta una operación dentro de una transacción de base de datos.
///     Sustituye al <c>ALICORPContexto</c> de .NET Framework: el commit se hace al
///     terminar sin excepción y el rollback es automático si la operación lanza,
///     así que no hay forma de olvidarse de ninguno de los dos.
/// </summary>
/// <remarks>
///     Todos los repositorios resueltos en el mismo ámbito comparten la conexión, y
///     en PostgreSQL la transacción pertenece a la conexión, así que una sola llamada
///     cubre a todos los repositorios que participen en la operación.
///     Las llamadas anidadas se unen a la transacción en curso en vez de abrir otra.
/// </remarks>
public interface ITransacciones
{
    void Ejecutar(Action operacion);

    T Ejecutar<T>(Func<T> operacion);

    Task EjecutarAsync(Func<Task> operacion);

    Task<T> EjecutarAsync<T>(Func<Task<T>> operacion);
}