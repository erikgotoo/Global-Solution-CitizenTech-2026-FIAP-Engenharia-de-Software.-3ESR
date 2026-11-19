using Microsoft.EntityFrameworkCore;
using Zeladoria.Core.Domain;

namespace Zeladoria.Infrastructure;

public class ZeladoriaDbContext(DbContextOptions<ZeladoriaDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<HistoricoStatus> Historicos => Set<HistoricoStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(u => u.Id);
            e.Property(u => u.Nome).IsRequired().HasMaxLength(Usuario.NomeMaximo);
            e.Property(u => u.Email).IsRequired().HasMaxLength(Usuario.EmailMaximo);
            e.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<HistoricoStatus>(e =>
        {
            e.ToTable("HistoricoStatus");
            e.HasKey(h => h.Id);
            e.Property(h => h.StatusAnterior).HasConversion<string>().HasMaxLength(20);
            e.Property(h => h.StatusNovo).HasConversion<string>().HasMaxLength(20);
            e.Property(h => h.Observacao).HasMaxLength(HistoricoStatus.ObservacaoMaxima);
            e.HasOne(h => h.Agente)
                .WithMany()
                .HasForeignKey(h => h.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(h => h.OcorrenciaId);
        });
    }
}
