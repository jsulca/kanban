using Kanban.Domain.Genericos.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class IntentoConfig : IEntityTypeConfiguration<Intento>
{
    public void Configure(EntityTypeBuilder<Intento> configuration)
    {
        configuration.ToTable("intento").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        configuration.Property(x => x.Usuario).HasColumnName("usuario");
        configuration.Property(x => x.Clave).HasColumnName("clave");
        configuration.Property(x => x.Descripcion).HasColumnName("descripcion");
        configuration.Property(x => x.FechaRegistro).HasColumnName("fecharegistro");
    }
}