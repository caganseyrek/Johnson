using Microsoft.EntityFrameworkCore;
using Johnson.Common.Models.DataStorage;

namespace Johnson.Infra.DataStorage;

public class JohnsonDbContext : DbContext
{
    public JohnsonDbContext(DbContextOptions<JohnsonDbContext> options) : base(options) {}

    public DbSet<EventAuditLog> EventAuditLogs { get; set; }
    public DbSet<InvalidatedAPIKey> InvalidatedAPIKey { get; set; }
    public DbSet<RegistryServiceEntry> RegistryServiceEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventAuditLog>(b => { b.HasKey(x => x.Id); });
        modelBuilder.Entity<InvalidatedAPIKey>(b => { b.HasKey(x => x.Key); });
        modelBuilder.Entity<RegistryServiceEntry>(b => { b.HasKey(x => x.Id); });
    }
}
