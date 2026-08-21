using Kanban.Domain.Genericos.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class EmpleadoConfig : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> configuration)
    {
        configuration.ToTable("empleado").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        configuration.Property(x => x.CargoId).HasColumnName("cargoid");
        configuration.Property(x => x.AreaId).HasColumnName("areaid");
        configuration.Property(x => x.Nombre).HasColumnName("nombre");
        configuration.Property(x => x.ApellidoPaterno).HasColumnName("apellidopaterno");
        configuration.Property(x => x.ApellidoMaterno).HasColumnName("apellidomaterno");
        configuration.Property(x => x.NroDocumento).HasColumnName("nrodocumento");
        configuration.Property(x => x.Correo).HasColumnName("correo");
        configuration.Property(x => x.Telefono).HasColumnName("telefono");

        configuration.Ignore(x => x.Cargo)
            .Ignore(x => x.Area);
    }
}