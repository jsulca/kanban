using System.Globalization;
using Kanban.Application;
using Kanban.Infrastructure;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

AppSettings.Configure(builder.Configuration);

builder.Services
    .AddControllersWithViews()
    // Web API 2 serializaba con los nombres tal cual (PascalCase) y no devolvía 400
    // automáticos: los controladores de /api validan a mano y siempre responden 200
    // con su propio código. Se conserva para no romper la app móvil.
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddHttpClient();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// La sesión ya solo guarda los mensajes de error entre la acción que falla y
// ErrorHandler; quién es el usuario lo dice la cookie de autenticación.
builder.Services.AddDistributedMemoryCache();

// Cachea unos minutos los permisos del rol, que no caben en la cookie.
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie de un día para la web, JWT para /api.
builder.Services.AddKanbanAuthentication();

var app = builder.Build();

// El Web.config declaraba <globalization uiCulture="es" culture="es-PE" />. Sin esto
// ASP.NET Core usa la cultura invariante y las fechas dd/MM/yyyy de los formularios
// se interpretarían al revés.
var culturaPeru = new CultureInfo("es-PE");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaPeru, new CultureInfo("es")),
    SupportedCultures = [culturaPeru],
    SupportedUICultures = [new CultureInfo("es")]
});

// Equivale a los customHeaders del Web.config (Kestrel no manda X-Powered-By).
app.Use(async (context, next) =>
{
    context.Response.Headers.XFrameOptions = "SAMEORIGIN";
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ErrorHandler/GetError");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/ErrorHandler/NotFound");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "areas",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();
