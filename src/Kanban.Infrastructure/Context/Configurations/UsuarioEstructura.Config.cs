using Kanban.Domain.Genericos.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class UsuarioEstructuraConfig : IEntityTypeConfiguration<UsuarioEstructura>
{
    public void Configure(EntityTypeBuilder<UsuarioEstructura> configuration)
    {
        configuration.ToTable("usuarioestructura").HasKey(x => new { x.UsuarioId, x.EstructuraId });

        configuration.Property(x => x.UsuarioId).HasColumnName("usuarioid");
        configuration.Property(x => x.EstructuraId).HasColumnName("estructuraid");

        configuration.Property(x => x.Acceso).HasColumnName("acceso");

        configuration.HasOne(x => x.Usuario).WithMany(x => x.Estructuras);
        configuration.Ignore(x => x.Estructura);
    }
}