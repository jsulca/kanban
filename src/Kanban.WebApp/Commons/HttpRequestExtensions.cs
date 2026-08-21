namespace Kanban.WebApp.Commons;

/// <summary>
///     ASP.NET Core no trae <c>Request.IsAjaxRequest()</c>; se replica mirando la
///     cabecera que envía jQuery, que es lo que comprueba la versión de MVC5.
/// </summary>
public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }
}
