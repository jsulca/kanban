namespace Kanban.WebApp.Commons;

public class Recaptcha
{
    public bool Success { get; set; }

    public string? Challenge_ts { get; set; }

    public string? Hostname { get; set; }

    public string? Action { get; set; }
}
