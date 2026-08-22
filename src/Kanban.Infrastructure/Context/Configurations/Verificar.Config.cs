using Kanban.Domain.Genericos.Verificaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Context.Configurations;

public class VerificarConfig : IEntityTypeConfiguration<Verificar>
{
    public void Configure(EntityTypeBuilder<Verificar> configuration)
    {
        configuration.ToTable("verificar").HasKey(x => x.Id);

        configuration.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        configuration.Property(x => x.EmpleadoId).HasColumnName("empleadoid");
        configuration.Property(x => x.VerificacionId).HasColumnName("verificacionid");
        configuration.Property(x => x.TableroId).HasColumnName("tableroid");
        configuration.Property(x => x.UsuarioId).HasColumnName("usuarioid");
        configuration.Property(x => x.EstructuraId).HasColumnName("estructuraid");

        configuration.Property(x => x.Encargado).HasColumnName("encargado");
        configuration.Property(x => x.Rom).HasColumnName("rom");
        configuration.Property(x => x.NroRom).HasColumnName("nrorom");
        configuration.Property(x => x.FechaRegistro).HasColumnName("fecharegistro");
        configuration.Property(x => x.Fortaleza).HasColumnName("fortaleza");
        configuration.Property(x => x.Oportunidad).HasColumnName("oportunidad");
        configuration.Property(x => x.PuntajeMaximo).HasColumnName("puntajemaximo");
        configuration.Property(x => x.PuntajeObtenido).HasColumnName("puntajeobtenido");
        configuration.Property(x => x.InstructivoEstandar).HasColumnName("instructivoestandar");
        configuration.Property(x => x.VP).HasColumnName("vp");
        configuration.Property(x => x.AreaId).HasColumnName("areaid");
        configuration.Property(x => x.IGP).HasColumnName("igp");

        configuration.Ignore(x => x.Empleado)
            .Ignore(x => x.Verificacion)
            .Ignore(x => x.Tablero)
            .Ignore(x => x.Estructura);
    }
}