using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class ColorController(IColorLogica colorLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetColor")
    {
        ViewBag.CallBack = callBack;

        Color model = new Color { };
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Color model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                colorLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetColor")
    {
        try
        {
            ViewBag.CallBack = callBack;

            Color model = colorLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Color model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                colorLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar()
    {
        try
        {
            List<Color> lista = colorLogica.Listar() ?? new List<Color>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.Descripcion,
                x.Hex,
                x.Rgba,
                x.Clase
            }));
            return Content(respuesta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(Color model)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Ingrese una descripción.");
    }

    #endregion
}
