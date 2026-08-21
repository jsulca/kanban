using Kanban.Domain.Genericos.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class AdjuntoConfig : IEntityTypeConfiguration<Adjunto>
{
    public void Configure(EntityTypeBuilder<Adjunto> configuration)
    {
        configuration.ToTable("adjunto").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        configuration.Property(x => x.Ruta).HasColumnName("ruta");
        configuration.Property(x => x.Nombre).HasColumnName("nombre");
        configuration.Property(x => x.TipoArchivo).HasColumnName("tipoarchivo");
        configuration.Property(x => x.Tamano).HasColumnName("tamano");
    }
}