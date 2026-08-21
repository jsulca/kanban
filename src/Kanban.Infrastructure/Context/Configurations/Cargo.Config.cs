using Kanban.Domain.Genericos.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class CargoConfig : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> configuration)
    {
        configuration.ToTable("cargo").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        configuration.Property(x => x.Codigo).HasColumnName("codigo");
        configuration.Property(x => x.Descripcion).HasColumnName("descripcion");
        configuration.Property(x => x.Activo).HasColumnName("activo");

        configuration.Ignore(x => x.Empleados);
    }
}