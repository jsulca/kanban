using Newtonsoft.Json;

namespace Kanban.WebApp.Commons;

/// <summary>
///     La sesión de ASP.NET Core solo guarda bytes y cadenas, no objetos como la de
///     .NET Framework. Estos ayudantes serializan a JSON. Hoy la sesión solo lleva los
///     mensajes de error entre la acción que falla y ErrorHandler: quién es el usuario
///     lo dice la cookie de autenticación (ver <see cref="UsuarioActual" />).
/// </summary>
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T? Get<T>(this ISession session, string key)
    {
        var json = session.GetString(key);
        return json is null ? default : JsonConvert.DeserializeObject<T>(json);
    }
}
