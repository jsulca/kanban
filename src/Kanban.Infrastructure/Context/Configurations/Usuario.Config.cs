using Kanban.Domain.Genericos.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> configuration)
    {
        configuration.ToTable("usuario").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        configuration.Property(x => x.RolId).HasColumnName("rolid");
        configuration.Property(x => x.EmpleadoId).HasColumnName("empleadoid");

        configuration.Property(x => x.Nombre).HasColumnName("nombre");
        configuration.Property(x => x.Clave).HasColumnName("clave");
        configuration.Property(x => x.Activo).HasColumnName("activo");
        configuration.Property(x => x.EstructuraId).HasColumnName("estructuraid");
        configuration.Property(x => x.Token).HasColumnName("token");
        configuration.Property(x => x.CambioClave).HasColumnName("cambioclave");
        configuration.Property(x => x.DiasVencimiento).HasColumnName("diasvencimiento");

        configuration.Ignore(x => x.Rol)
            .Ignore(x => x.Empleado)
            .Ignore(x => x.Estructura);
    }
}