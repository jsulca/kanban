using Kanban.Domain.Genericos.Verificacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class PlanAccionConfig : IEntityTypeConfiguration<PlanAccion>
{
    public void Configure(EntityTypeBuilder<PlanAccion> configuration)
    {
        configuration.ToTable("planaccion").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        configuration.Property(x => x.VerificarId).HasColumnName("verificarid");
        configuration.Property(x => x.Descripcion).HasColumnName("descripcion");

        configuration.HasOne(x => x.Verificar).WithMany(x => x.PlanesAccion);

        configuration.Ignore(x => x.Compromiso);
    }
}