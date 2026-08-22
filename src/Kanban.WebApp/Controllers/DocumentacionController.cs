using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Controllers;

/// <summary>
///     Manual de usuario. Va con <c>NoValidarAccion</c> para que lo pueda leer
///     cualquiera que haya entrado a la aplicación: no exige permiso de página, así que
///     no hace falta darlo de alta en los maestros de Seguridad.
/// </summary>
[SafetyFilter(NoValidarAccion = true)]
public class DocumentacionController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Maestros()
    {
        return View();
    }

    public ActionResult Seguridad()
    {
        return View();
    }

    public ActionResult Compromisos()
    {
        return View();
    }

    public ActionResult Confirmaciones()
    {
        return View();
    }
}
