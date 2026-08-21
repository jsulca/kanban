namespace Kanban.WebApp.Commons;

/// <summary>
///     Sustituye al <c>AppSettings</c> que leía de <c>ConfigurationManager</c>. Se
///     rellena una sola vez al arrancar (<see cref="Configure" />) desde la sección
///     "AppSettings" de appsettings.json; se mantiene estático y con los nombres
///     originales para no tocar los cientos de sitios que lo consultan así.
/// </summary>
public static class AppSettings
{
    public static string RutaArchivo { get; private set; } = "";

    public static string NombreEmpresa { get; private set; } = "ALICORP";

    public static string AZURE_BLOB_CONNECTION { get; private set; } = "";

    public static string AZURE_BLOB_CONTAINER { get; private set; } = "";

    public static bool AZURE_BLOB_ACTIVO { get; private set; }

    public static string URL_BASE { get; private set; } = "";

    public static string JWT_SECRET_KEY { get; private set; } = "";

    public static string JWT_AUDIENCE_TOKEN { get; private set; } = "";

    public static string JWT_ISSUER_TOKEN { get; private set; } = "";

    public static void Configure(IConfiguration configuration)
    {
        var seccion = configuration.GetSection("AppSettings");

        RutaArchivo = seccion["FilePath"] ?? "";
        NombreEmpresa = seccion["NombreEmpresa"] ?? "ALICORP";
        AZURE_BLOB_CONNECTION = seccion["AZURE_BLOB_CONNECTION"] ?? "";
        AZURE_BLOB_CONTAINER = seccion["AZURE_BLOB_CONTAINER"] ?? "";
        AZURE_BLOB_ACTIVO = (seccion["AZURE_BLOB_ACTIVO"] ?? "").Equals("true", StringComparison.OrdinalIgnoreCase);
        URL_BASE = seccion["URL_BASE"] ?? "";
        JWT_SECRET_KEY = seccion["JWT_SECRET_KEY"] ?? "";
        JWT_AUDIENCE_TOKEN = seccion["JWT_AUDIENCE_TOKEN"] ?? "";
        JWT_ISSUER_TOKEN = seccion["JWT_ISSUER_TOKEN"] ?? "";
    }
}
