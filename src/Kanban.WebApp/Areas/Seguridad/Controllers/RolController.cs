using Newtonsoft.Json;
using System.Net;

namespace Kanban.WebApp.Areas.Seguridad.Controllers;

[SafetyFilter(NoValidarAccion = true)]
[Area("Seguridad")]
public class RolController(IRolLogica rolLogica, IMenuLogica menuLogica, IPaginaLogica paginaLogica) : Controller
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    [SafetyFilter]
    public ActionResult Nuevo()
    {
        try
        {
            Rol model = new Rol { Activo = true };

            List<Menu> menus = menuLogica.Listar() ?? new List<Menu>();
            TreeModel root = new TreeModel() { id = "0", text = AppSettings.NombreEmpresa, icon = "fa fa-sitemap", state = new { opened = true } };
            LlenarArbol(root, menus);
            ViewBag.MenuJson = JsonConvert.SerializeObject(root);

            ViewBag.Paginas = paginaLogica.Listar(true) ?? new List<Pagina>();

            return View(model);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Rol model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                rolLogica.Guardar(model);
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

    [SafetyFilter]
    public ActionResult Editar(int id)
    {
        try
        {

            Rol model = rolLogica.BuscarPorId(id, true);

            List<Menu> menus = menuLogica.Listar();
            TreeModel root = new TreeModel() { id = "0", text = AppSettings.NombreEmpresa, icon = "fa fa-sitemap", state = new { opened = true } };
            LlenarArbol(root, menus ?? new List<Menu>());
            ViewBag.MenuJson = JsonConvert.SerializeObject(root);

            ViewBag.Paginas = paginaLogica.Listar(true) ?? new List<Pagina>();

            ViewBag.RolMenu = JsonConvert.SerializeObject(model.Menus ?? new List<RolMenu>());

            return View(model);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    public ActionResult Editar(Rol model)
    {
        try
        {
            ValidarEntidad(model);
            if (ModelState.IsValid)
            {
                rolLogica.Actualizar(model);
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

    public ActionResult ListarPorPagina(RolFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = rolLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;
            if (lista == null) lista = new List<Rol>();
            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    x.Activo
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

    #region Metodos y Funciones

    [NonAction]
    private void ValidarEntidad(Rol item)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(item.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un nombre");
    }

    [NonAction]
    private void LlenarArbol(TreeModel padre, List<Menu> menus)
    {
        List<Menu> subMenus = menus.Where(x => padre.id.Equals(x.PadreId?.ToString() ?? "0")).ToList();

        if (subMenus != null)
        {
            TreeModel nodo = null;
            foreach (Menu menu in subMenus)
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
