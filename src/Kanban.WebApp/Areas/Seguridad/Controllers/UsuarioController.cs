using Kanban.SharedKernel;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Areas.Seguridad.Controllers;

[SafetyFilter(NoValidarAccion = true)]
[Area("Seguridad")]
public class UsuarioController(
    IUsuarioLogica usuarioLogica,
    IRolLogica rolLogica,
    IEmpleadoLogica empleadoLogica,
    IEstructuraLogica estructuraLogica,
    IIntentoLogica intentoLogica,
    IConfiguracionLogica configuracionLogica) : Controller
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo()
    {
        try
        {

            Usuario model = new Usuario
            {
                Activo = true,
                DiasVencimiento = configuracionLogica.Buscar(ConfiguracionMaestro.RENOVACION_CLAVE).Dias
            };

            List<Empleado> empleados = empleadoLogica.Listar() ?? new List<Empleado>();
            List<Rol> roles = rolLogica.Listar() ?? new List<Rol>();
            List<Estructura> estructuras = estructuraLogica.Listar();

            empleados.ForEach(x => x.Nombre = string.Format("{0} {1} {2}", x.ApellidoPaterno, x.ApellidoMaterno, x.Nombre));

            ViewBag.Empleados = empleados;
            ViewBag.Roles = roles;

            TreeModel root = new TreeModel() { id = "0", text = AppSettings.NombreEmpresa, icon = "fa fa-sitemap", state = new { opened = true } };
            LlenarArbol(root, estructuras);
            ViewBag.EstructuraJson = JsonConvert.SerializeObject(root);

            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Usuario model, bool activarCaducidad)
    {
        try
        {

            ValidarEntidad(model, activarCaducidad);
            if (ModelState.IsValid)
            {
                if (usuarioLogica.ExisteUsuario(model.Id, model.Nombre))
                    ModelState.AddModelError("", "Ya existe un usuario con ese nombre");
            }

            if (ModelState.IsValid)
            {
                if (model.RolId != RolMaestro.ADMINISTRADOR || activarCaducidad)
                    model.DiasVencimiento = configuracionLogica.Buscar(ConfiguracionMaestro.RENOVACION_CLAVE).Dias;
                else model.DiasVencimiento = null;

                model.Clave = CryptographyHelper.Encrypt(model.Clave);
                //usuarioLogica.Guardar(model);
                usuarioLogica.GuardarEF(model);
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

            Usuario model = usuarioLogica.BuscarPorId(id, true);

            if(model != null) 
                model.DiasVencimiento = configuracionLogica.Buscar(ConfiguracionMaestro.RENOVACION_CLAVE).Dias;

            List<Empleado> empleados = empleadoLogica.Listar() ?? new List<Empleado>();
            List<Rol> roles = rolLogica.Listar() ?? new List<Rol>();
            List<Estructura> estructuras = estructuraLogica.Listar();

            empleados.ForEach(x => x.Nombre = string.Format("{0} {1} {2}", x.ApellidoPaterno, x.ApellidoMaterno, x.Nombre));

            ViewBag.Empleados = empleados;
            ViewBag.Roles = roles;

            TreeModel root = new TreeModel() { id = "0", text = AppSettings.NombreEmpresa, icon = "fa fa-sitemap", state = new { opened = true } };
            LlenarArbol(root, estructuras);
            ViewBag.EstructuraJson = JsonConvert.SerializeObject(root);
            ViewBag.Estructuras = JsonConvert.SerializeObject(model.Estructuras);

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
    public ActionResult Editar(Usuario model, bool activarCaducidad)
    {
        try
        {

            ValidarEntidad(model, activarCaducidad);
            if (ModelState.IsValid)
            {
                if (usuarioLogica.ExisteUsuario(model.Id, model.Nombre))
                    ModelState.AddModelError("", "Ya existe un usuario con ese nombre");
            }
            if (ModelState.IsValid)
            {
                if (model.RolId != RolMaestro.ADMINISTRADOR || activarCaducidad)
                    model.DiasVencimiento = configuracionLogica.Buscar(ConfiguracionMaestro.RENOVACION_CLAVE).Dias;
                else model.DiasVencimiento = null;

                if (!string.IsNullOrWhiteSpace(model.Clave))
                    model.Clave = CryptographyHelper.Encrypt(model.Clave);

                usuarioLogica.Actualizar(model);

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

    public ActionResult ListarPorPagina(UsuarioFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = usuarioLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;
            if (lista == null) lista = new List<Usuario>();
            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    x.Activo,
                    Empleado = new
                    {
                        x.Empleado.Nombre,
                        x.Empleado.ApellidoPaterno,
                        x.Empleado.ApellidoMaterno
                    },
                    Rol = new { x.Rol.Nombre }
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

    public ActionResult IntentosFallidos(int id)
    {
        try
        {

            int cantidadRegistros = 20;
            Usuario usuario = usuarioLogica.BuscarPorId(id);
            ViewBag.Lista = intentoLogica.Listar(usuario.Nombre, cantidadRegistros);
            ViewBag.CantidadRegistros = 20;

            return PartialView("_IntentosFallidos", usuario);
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
    private void ValidarEntidad(Usuario model, bool activarCaducidad)
    {
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese el nombre.");
        if (string.IsNullOrEmpty(model.Clave) && model.Id == 0) ModelState.AddModelError("Clave", "Ingrese la clave.");
        else if (!string.IsNullOrEmpty(model.Clave))
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            //var hasMinimum8Chars = new Regex(@".{8,}");
            //TODO: Agregar 10 Caracteres
            var hasMinimum10Chars = new Regex(@".{10,}");
            var hasSpecialCharacter = new Regex(@"[!@#$%^&*]+");

            if (!hasUpperChar.IsMatch(model.Clave)) ModelState.AddModelError("Clave", "La clave debe contener un carácter en mayuscula");
            if (!hasNumber.IsMatch(model.Clave)) ModelState.AddModelError("Clave", "La clave debe contener un número");
            if (!hasSpecialCharacter.IsMatch(model.Clave)) ModelState.AddModelError("Clave", "La clave debe contener al menos una carácter especial.");
            if (!hasMinimum10Chars.IsMatch(model.Clave)) ModelState.AddModelError("Clave", "La clave de contener como minimo 10 caracteres");
        }

        if (model.EmpleadoId <= 0) ModelState.AddModelError("EmpleadoId", "Seleccione un empleado.");
        if (model.RolId <= 0) ModelState.AddModelError("RolId", "Seleccione un rol.");
        if (model.RolId != RolMaestro.ADMINISTRADOR && !activarCaducidad) ModelState.AddModelError("RolId", "Un rol distinto a ADMINISTRADOR debe tener un tiempo de caducidad.");
        if (model.EstructuraId <= 0) ModelState.AddModelError("EstructuraId", "Seleccione un nivel.");
        if (model.Estructuras == null || model.Estructuras.Count == 0) ModelState.AddModelError("Estructuras", "No se encuentra asociado a muchas estructuras.");
        else
        {
            int i = 0;
            foreach (var item in model.Estructuras)
                if (item.EstructuraId <= 0)
                    ModelState.AddModelError("Estructuras_" + i, string.Format("La estructura Nº {0} no tiene un identificador.", i));
        }
    }

    [NonAction]
    private void LlenarArbol(TreeModel padre, List<Estructura> items)
    {
        List<Estructura> subMenus = items.Where(x => padre.id.Equals(x.PadreId?.ToString() ?? "0")).ToList();

        if (subMenus != null)
        {
            TreeModel nodo = null;
            foreach (var item in subMenus)
            {
                nodo = new TreeModel()
                {
                    id = item.Id.ToString(),
                    text = item.Descripcion,
                    icon = item.Tablero ? "fa fa-table text-success" : "fa fa-boxes",
                    state = new { opened = true }
                };
                LlenarArbol(nodo, items);
                padre.children.Add(nodo);
            }
        }
    }

    #endregion
}
