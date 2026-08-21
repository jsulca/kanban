namespace Kanban.WebApp.Commons;

/// <summary>
///     Dónde se guardan los adjuntos: en Azure Blob si <c>AZURE_BLOB_ACTIVO</c> está
///     activo y, si no, en la carpeta de <c>AppSettings:FilePath</c>.
///     <para>
///         Antes esta lógica estaba duplicada en el controlador web y en el de la API,
///         y con el blob apagado el archivo se perdía sin avisar porque no había
///         alternativa: el <c>if</c> no tenía <c>else</c>.
///     </para>
/// </summary>
public static class AlmacenArchivos
{
    public static Adjunto Guardar(IFormFile archivo)
    {
        var nombre = NombreUnico(archivo.FileName);

        var ruta = AppSettings.AZURE_BLOB_ACTIVO
            ? GuardarEnBlob(archivo, nombre)
            : GuardarEnDisco(archivo, nombre);

        return new Adjunto
        {
            Nombre = nombre,
            Ruta = ruta,
            Tamano = Convert.ToInt32(archivo.Length),
            TipoArchivo = archivo.ContentType
        };
    }

    /// <summary>
    ///     Indica si la ruta guardada apunta al blob (una URL) o a una carpeta del
    ///     servidor. En la base de datos conviven las dos formas.
    /// </summary>
    public static bool EsUrl(string? ruta)
    {
        return Uri.TryCreate(ruta, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    ///     El nombre era solo la fecha al segundo, así que dos personas subiendo una
    ///     foto a la vez chocaban. Se le añade un sufijo corto.
    /// </summary>
    private static string NombreUnico(string nombreOriginal)
    {
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        return $"{DateTime.Now:yyyyMMddHHmmss}_{sufijo}{Path.GetExtension(nombreOriginal)}";
    }

    private static string GuardarEnBlob(IFormFile archivo, string nombre)
    {
        using var origen = archivo.OpenReadStream();
        return BlobStorage.UploadFile(origen, nombre);
    }

    private static string GuardarEnDisco(IFormFile archivo, string nombre)
    {
        if (string.IsNullOrWhiteSpace(AppSettings.RutaArchivo))
            throw new Exception("Falta configurar AppSettings:FilePath, la carpeta donde se guardan los adjuntos.");

        Directory.CreateDirectory(AppSettings.RutaArchivo);

        var ruta = Path.Combine(AppSettings.RutaArchivo, nombre);

        // CreateNew y no Create: si el nombre ya existiera preferimos el error a
        // machacar el adjunto de otro compromiso.
        using (var destino = new FileStream(ruta, FileMode.CreateNew))
        {
            archivo.CopyTo(destino);
        }

        return ruta;
    }
}
