using System.Net;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Controllers;

public abstract class AlicorpController : Controller
{
    public ActionResult Validation(string? mensaje = null)
    {
        Response.StatusCode = (int)HttpStatusCode.BadRequest;

        if (!ModelState.IsValid)
            HttpContext.Session.Set("Errores", ModelState.Keys
                .SelectMany(key => ModelState[key]!.Errors)
                .Select(k => k.ErrorMessage)
                .ToArray());

        if (!string.IsNullOrEmpty(mensaje)) HttpContext.Session.SetString("Mensaje", mensaje);

        if (Request.IsAjaxRequest()) return PartialView("_Error");
        return View("Error");
    }
}