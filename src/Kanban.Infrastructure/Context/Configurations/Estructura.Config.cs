using Kanban.Domain.Genericos.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class EstructuraConfig : IEntityTypeConfiguration<Estructura>
{
    public void Configure(EntityTypeBuilder<Estructura> configuration)
    {
        configuration.ToTable("estructura").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        configuration.Property(x => x.PadreId).HasColumnName("padreid");

        configuration.Property(x => x.Codigo).HasColumnName("codigo");
        configuration.Property(x => x.Descripcion).HasColumnName("descripcion");
        configuration.Property(x => x.Tablero).HasColumnName("tablero");

        configuration.Ignore(x => x.Instancias)
            .Ignore(x => x.Areas)
            .Ignore(x => x.Empleados)
            .Ignore(x => x.Sostenibilidades)
            .Ignore(x => x.Compromisos);
    }
}