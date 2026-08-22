using System.Text.RegularExpressions;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.SharedKernel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Kanban.WebApp.Commons;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

public class AccountController(
    IUsuarioLogica usuarioLogica,
    IIntentoLogica intentoLogica) : Controller
{
    #region Acciones

    public ActionResult Login(string? ReturnUrl = null)
    {
        var model = new Usuario();
        ViewBag.ReturnUrl = ReturnUrl;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Login(Usuario model, string? ReturnUrl, string? recaptcha)
    {
        try
        {
            ValidarLogin(model);
            if (string.IsNullOrEmpty(recaptcha)) ModelState.AddModelError("Captcha", "Es necesario ingresar el captcha");
            if (ModelState.IsValid)
            {
                var textoTemporal = TempData["Captcha"]?.ToString();
                if (recaptcha != textoTemporal)
                    ModelState.AddModelError("", "El texto ingresado no coincide con la imagen.");
            }

            if (ModelState.IsValid)
            {
                model.Clave = CryptographyHelper.Encrypt(model.Clave!);
                var usuario = usuarioLogica.BuscarLogin(model.Nombre!, model.Clave);

                if (usuario == null)
                {
                    ModelState.AddModelError("Usuario", "Usuario y/o clave incorrecta.");
                    intentoLogica.Guardar(new Intento
                    {
                        Usuario = model.Nombre,
                        Clave = model.Clave,
                        Descripcion = "Usuario y/o clave incorrecta. A través de la página web."
                    });
                }
                else if (!usuario.Activo)
                {
                    ModelState.AddModelError("Activo", "El usuario no se encuentra activo.");
                    intentoLogica.Guardar(new Intento
                    {
                        Usuario = model.Nombre,
                        Clave = model.Clave,
                        Descripcion = "El usuario no se encuentra activo. A través de la página web."
                    });
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ReturnUrl = ReturnUrl;
                    return View(model);
                }

                //TODO: REDIRECCIONAR A CAMBIAR LA CLAVE POR EL REINICIO DEL MISMO
                if (!usuario!.CambioClave.HasValue)
                {
                    TempData["id"] = usuario.Id;
                    TempData["motivo"] = "REINICIO DE CLAVE";
                    return RedirectToAction("ChangePassword");
                }

                if (usuario.DiasVencimiento.HasValue &&
                    DateTime.Today > usuario.CambioClave.Value.AddDays(usuario.DiasVencimiento.Value))
                {
                    TempData["id"] = usuario.Id;
                    TempData["motivo"] = "CLAVE VENCIDA";
                    return RedirectToAction("ChangePassword");
                }

                await IniciarSesion(usuario);

                return Redirect(ReturnUrl ?? "/");
            }

            ViewBag.ReturnUrl = ReturnUrl;
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ModelState.AddModelError("", ex.Message);
            ModelState.AddModelError("", JsonConvert.SerializeObject(ex));
            return View(model);
        }
    }

    public async Task<ActionResult> Logout()
    {
        var user = HttpContext.GetUser();
        if (user != null) HttpContext.OlvidarUsuario(user.UserId);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    public ActionResult ChangePassword()
    {
        var model = new AccountModel.ChangePassword
        {
            Id = (int)(TempData["id"] ?? 0)
        };

        ViewBag.Motivo = TempData["motivo"] ?? "";

        if (model.Id == 0) return RedirectToAction("Login");

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ChangePassword(AccountModel.ChangePassword model, string? motivo = null)
    {
        try
        {
            Usuario? usuario = null;
            var usuarioCambiarClave = new Usuario();

            ValidarChangePassword(model);
            if (ModelState.IsValid)
            {
                usuario = usuarioLogica.BuscarPorId(model.Id, true);
                usuarioCambiarClave = model.Get();

                //TODO: REGISTRAR CAMBIO DE CLAVE
                usuarioCambiarClave.CambioClave = DateTime.Today;
                usuarioCambiarClave.Clave = CryptographyHelper.Encrypt(usuarioCambiarClave.Clave!);
                if (usuarioCambiarClave.Clave.Equals(usuario?.Clave))
                    ModelState.AddModelError("Clave", "Debe ingresar una clave distinta a la anterior.");
            }

            if (ModelState.IsValid)
            {
                //TODO: REGISTRAR CAMBIO DE CLAVE
                usuarioLogica.CambiarClave(usuarioCambiarClave);

                await IniciarSesion(usuario!);

                return Redirect("/");
            }

            ViewBag.Motivo = motivo ?? "";
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Motivo = motivo ?? "";
            return View(model);
        }
    }

    public ActionResult Captcha()
    {
        var captcha = new CaptchaHelper();
        TempData["Captcha"] = captcha.Texto;

        return File(captcha.ToByteArray(), "image/svg+xml");
    }

    #endregion

    #region Metodos y Funciones

    /// <summary>
    ///     Firma la cookie de autenticación. <c>IsPersistent</c> hace que sobreviva a
    ///     cerrar el navegador; la caducidad la pone <c>ExpireTimeSpan</c> (un día).
    /// </summary>
    [NonAction]
    private Task IniciarSesion(Usuario usuario)
    {
        return HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            UsuarioActual.CrearPrincipal(usuario),
            new AuthenticationProperties { IsPersistent = true });
    }

    [NonAction]
    private void ValidarLogin(Usuario model)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese el usuario.");
        if (string.IsNullOrWhiteSpace(model.Clave)) ModelState.AddModelError("Clave", "Ingrese la clave.");
    }

    [NonAction]
    private void ValidarChangePassword(AccountModel.ChangePassword model)
    {
        ModelState.Clear();
        if (model.Id <= 0) ModelState.AddModelError("Id", "El identificador del usuario no es válido.");
        if (string.IsNullOrWhiteSpace(model.NuevaClave))
        {
            ModelState.AddModelError("Clave", "Ingrese la clave.");
        }
        else
        {
            if (!model.NuevaClave.Equals(model.ConfirmacionClave))
            {
                ModelState.AddModelError("Clave", "Las claves no coinciden");
            }
            else
            {
                var hasNumber = new Regex("[0-9]+");
                var hasUpperChar = new Regex("[A-Z]+");
                //TODO: Agregar 10 Caracteres
                var hasMinimum10Chars = new Regex(".{10,}");
                var hasSpecialCharacter = new Regex("[!@#$%^&*]+");

                if (!hasUpperChar.IsMatch(model.NuevaClave))
                    ModelState.AddModelError("Clave", "La clave debe contener un carácter en mayuscula");
                if (!hasNumber.IsMatch(model.NuevaClave))
                    ModelState.AddModelError("Clave", "La clave debe contener un número");
                if (!hasSpecialCharacter.IsMatch(model.NuevaClave))
                    ModelState.AddModelError("Clave", "La clave debe contener al menos una carácter especial.");
                if (!hasMinimum10Chars.IsMatch(model.NuevaClave))
                    ModelState.AddModelError("Clave", "La clave de contener como minimo 10 caracteres");
            }
        }
    }

    #endregion
}
