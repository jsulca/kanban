using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Domain.Genericos.Verificaciones;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Context;

public class EFContexto : DbContext
{
    public EFContexto(DbContextOptions<EFContexto> options) : base(options)
    {
    }

    public DbSet<Empleado> Empleado { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<UsuarioEstructura> UsuarioEstructura { get; set; }

    public DbSet<Verificar> Verificar { get; set; }
    public DbSet<VerificarRespuesta> VerificarRespuesta { get; set; }
    public DbSet<PlanAccion> PlanAccion { get; set; }

    public DbSet<Estructura> Estructura { get; set; }
    public DbSet<Cargo> Cargo { get; set; }

    public DbSet<Adjunto> Adjunto { get; set; }
    public DbSet<Intento> Intento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFContexto).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
