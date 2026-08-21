using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kanban.WebApp.Commons;

/// <summary>
///     Filtro de acceso: exige sesión iniciada y que el rol del usuario tenga
///     concedida la página (área + controlador + acción). Cuando la concede, deja en
///     <c>ViewBag.Controles</c> los controles permitidos de esa página.
/// </summary>
public class SafetyFilter : ActionFilterAttribute
{
    /// <summary>
    ///     Para acciones auxiliares que solo requieren sesión, sin permiso de página.
    /// </summary>
    public bool NoValidarAccion { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var request = filterContext.HttpContext.Request;
        var user = filterContext.HttpContext.GetUser();

        var nuevaURL = $"{AppSettings.URL_BASE}{request.Path}{request.QueryString}";

        var area = filterContext.RouteData.Values["area"]?.ToString() ?? "";
        var descriptor = (ControllerActionDescriptor)filterContext.ActionDescriptor;
        var controller = descriptor.ControllerName;
        var action = descriptor.ActionName;

        if (user == null)
        {
            if (request.IsAjaxRequest())
                filterContext.Result = new RedirectToActionResult("PartialError", "Home",
                    new { area = "", isSesion = true });
            else
                filterContext.Result = new RedirectToActionResult("Login", "Account",
                    new { area = "", ReturnUrl = nuevaURL });
        }
        else
        {
            var pagina = user.Pages.FirstOrDefault(x =>
                (x.Area ?? "") == area && x.Controlador == controller && x.Accion == action);

            if (pagina != null)
            {
                var controles = user.Controls.Where(x => x.PaginaId == pagina.Id).ToList();
                if (filterContext.Controller is Controller controlador) controlador.ViewBag.Controles = controles;
            }
            else if (NoValidarAccion)
            {
            }
            else
            {
                filterContext.Result = new RedirectToActionResult("PageNotFound", "Home", new { area = "" });
            }
        }

        base.OnActionExecuting(filterContext);
    }
}
