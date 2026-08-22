using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Controllers;

public class ErrorHandlerController : Controller
{
    #region Acciones

    public ActionResult Index()
    {
        return Content("Error");
    }

    public new ActionResult NotFound()
    {
        return View();
    }

    public ActionResult Validation()
    {
        Response.StatusCode = 400;
        if (Request.IsAjaxRequest())
        {
            ViewBag.Message = HttpContext.Session.GetString("Mensaje");
            ViewBag.Errores = HttpContext.Session.Get<string[]>("Errores");
            return PartialView("_Error");
        }

        return View("Error");
    }

    public ActionResult GetError()
    {
        ViewBag.Message = HttpContext.Session.GetString("Mensaje");
        ViewBag.Errores = HttpContext.Session.Get<string[]>("Errores");

        if (Request.IsAjaxRequest()) return PartialView("_Error");
        return View("Error");
    }

    #endregion
}