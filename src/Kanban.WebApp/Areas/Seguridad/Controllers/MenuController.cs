using Newtonsoft.Json;
using System.Net;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Areas.Seguridad.Controllers;

[SafetyFilter(NoValidarAccion = true)]
[Area("Seguridad")]
public class MenuController(IMenuLogica menuLogica) : Controller
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        try
        {
            List<Menu> menus = menuLogica.Listar();

            TreeModel root = new TreeModel() { id = "0", text = AppSettings.NombreEmpresa, icon = "fa fa-sitemap", state = new { opened = true } };

            LlenarArbol(root, menus ?? new List<Menu>());

            ViewBag.MenuJson = JsonConvert.SerializeObject(root);

            return View();
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    public ActionResult Nuevo(int id)
    {
        Menu model = new Menu { PadreId = id };
        if (id == 0) model.Tipo = TipoMenu.HEADER;
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Menu model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                menuLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Error");
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    public ActionResult Editar(int id)
    {
        try
        {
            Menu model = menuLogica.BuscarPorId(id);
            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    [HttpPost]
    public ActionResult Editar(Menu model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                menuLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("_Error");
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void ValidarEntidad(Menu item)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(item.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un nombre.");
        if (item.Tipo < TipoMenu.NORMAL || item.Tipo > TipoMenu.HEADER) ModelState.AddModelError("Tipo", "Seleccione un tipo de menu");
        else
        {
            switch (item.Tipo)
            {
                case TipoMenu.NORMAL:
                    if (string.IsNullOrWhiteSpace(item.Url)) ModelState.AddModelError("Url", "Ingrese una url.");
                    break;
                case TipoMenu.COLLAPSE:
                    if (string.IsNullOrWhiteSpace(item.Icono)) ModelState.AddModelError("Icono", "Ingrese un icono.");
                    break;
            }
        }
    }

    [NonAction]
    private void LlenarArbol(TreeModel padre, List<Menu> menus)
    {
        List<Menu> subMenus = menus.Where(x => padre.id.Equals(x.PadreId?.ToString() ?? "0")).ToList();

        if (subMenus != null)
        {
            TreeModel nodo = null;
            foreach (Menu menu in subMenus.OrderBy(x => x.Orden))
            {
                nodo = new TreeModel()
                {
                    id = menu.Id.ToString(),
                    text = menu.Nombre,
                    icon = menu.Icono ?? "",
                    state = new { opened = true }
                };
                LlenarArbol(nodo, menus);
                padre.children.Add(nodo);
            }
        }
    }
    #endregion
}
