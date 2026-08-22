using Kanban.Domain.Genericos.Verificaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class VerificarRespuestaConfig : IEntityTypeConfiguration<VerificarRespuesta>
{
    public void Configure(EntityTypeBuilder<VerificarRespuesta> configuration)
    {
        configuration.ToTable("verificarrespuesta").HasKey(x => new { x.VerificarId, x.CategoriaId, x.PreguntaId });

        configuration.Property(x => x.VerificarId).HasColumnName("verificarid");
        configuration.Property(x => x.CategoriaId).HasColumnName("categoriaid");
        configuration.Property(x => x.PreguntaId).HasColumnName("preguntaid");
        configuration.Property(x => x.Descripcion).HasColumnName("descripcion");
        configuration.Property(x => x.Valor).HasColumnName("valor");

        configuration.HasOne(x => x.Verificar).WithMany(x => x.Respuestas);

        configuration.Ignore(x => x.Categoria)
            .Ignore(x => x.Pregunta);
    }
}