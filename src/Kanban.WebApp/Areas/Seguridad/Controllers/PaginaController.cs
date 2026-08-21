using Newtonsoft.Json;
using System.Net;

namespace Kanban.WebApp.Areas.Seguridad.Controllers;

[SafetyFilter(NoValidarAccion = true)]
[Area("Seguridad")]
public class PaginaController(IPaginaLogica paginaLogica) : Controller
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo()
    {
        Pagina model = new Pagina();
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Pagina model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                paginaLogica.Guardar(model);
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
            Pagina model = paginaLogica.BuscarPorId(id, true);
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
    public ActionResult Editar(Pagina model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                paginaLogica.Actualizar(model);
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

    public ActionResult ListarPorPagina(PaginaFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = paginaLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;
            if (lista == null) lista = new List<Pagina>();
            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    Area = x.Area ?? "",
                    x.Controlador,
                    x.Accion
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    #endregion

    #region Metodo y Funciones

    [NonAction]
    public void ValidarEntidad(Pagina item)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(item.Nombre)) ModelState.AddModelError("Nombre", "Ingresar el nombre.");
        if (string.IsNullOrWhiteSpace(item.Controlador)) ModelState.AddModelError("Controlador", "Ingresar el controlador.");
        if (string.IsNullOrWhiteSpace(item.Accion)) ModelState.AddModelError("Accion", "Ingresar la accion.");

        if (item.Controles != null)
        {
            int i = 1;
            foreach (Control control in item.Controles)
            {
                if (string.IsNullOrWhiteSpace(control.Nombre)) ModelState.AddModelError("Controles", string.Concat("El control Nº ", i, " no tiene un nombre."));
                i++;
            }
        }
    }

    #endregion
}
