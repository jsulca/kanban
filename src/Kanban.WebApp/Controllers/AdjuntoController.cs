using Kanban.WebApp.Commons;

namespace Kanban.WebApp.Controllers;

public class AdjuntoController(
    IAdjuntoLogica adjuntoLogica,
    IWebHostEnvironment entorno,
    IHttpClientFactory clientFactory) : Controller
{
    #region Acciones

    public async Task<ActionResult> Index(int id)
    {
        try
        {
            var adjunto = adjuntoLogica.Buscar(id) ?? throw new Exception("No existe el adjunto.");
            var contenido = await AbrirAsync(adjunto);

            return File(contenido, adjunto.TipoArchivo ?? "application/octet-stream", adjunto.Nombre);
        }
        catch (Exception)
        {
            return NoEncontrada();
        }
    }

    public async Task<ActionResult> VerFoto(int id)
    {
        return await VerImagen(id);
    }

    public async Task<ActionResult> VerFotoBlob(int id)
    {
        return await VerImagen(id);
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private async Task<ActionResult> VerImagen(int id)
    {
        try
        {
            var adjunto = adjuntoLogica.Buscar(id) ?? throw new Exception("No existe el adjunto.");
            var contenido = await AbrirAsync(adjunto);

            return File(contenido, "image/jpeg", "imagen");
        }
        catch (Exception)
        {
            return NoEncontrada();
        }
    }

    /// <summary>
    ///     Abre el adjunto esté donde esté: en la carpeta del servidor o en el blob.
    ///     En la base de datos conviven las dos formas, porque el destino ha cambiado
    ///     con el tiempo.
    /// </summary>
    [NonAction]
    private async Task<Stream> AbrirAsync(Adjunto adjunto)
    {
        if (!AlmacenArchivos.EsUrl(adjunto.Ruta)) return System.IO.File.OpenRead(adjunto.Ruta!);

        if (AppSettings.AZURE_BLOB_ACTIVO) return BlobStorage.DownloadFile(adjunto.Nombre!);

        // Quedan filas apuntando al blob pero ya no hay credenciales configuradas:
        // se intenta por la URL, que solo funcionará si el contenedor es público.
        return await clientFactory.CreateClient().GetStreamAsync(adjunto.Ruta);
    }

    [NonAction]
    private ActionResult NoEncontrada()
    {
        return PhysicalFile(Path.Combine(entorno.WebRootPath, "images", "notfound.png"), "image/png");
    }

    #endregion
}
