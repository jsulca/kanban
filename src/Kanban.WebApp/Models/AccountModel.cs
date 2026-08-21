namespace Kanban.WebApp.Models;

public struct AccountModel
{
    public class Get
    {
        public string? Nombre { get; set; }
        public string? Clave { get; set; }
        public string? Recaptcha { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
    }

    public class ChangePassword
    {
        public int Id { get; set; }
        public string? NuevaClave { get; set; }
        public string? ConfirmacionClave { get; set; }

        public Usuario Get() => new Usuario() { Id = Id, Clave = NuevaClave };
    }
}
