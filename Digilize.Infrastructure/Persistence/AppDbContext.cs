using Microsoft.EntityFrameworkCore;
using Digilize.Domain.Entities;
using System.Text.Json;

namespace Digilize.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>().Property(a => a.EntityName).IsRequired();
            modelBuilder.Entity<AuditLog>().Property(a => a.Operation).IsRequired();
            modelBuilder.Entity<AuditLog>().Property(a => a.EntityId).IsRequired();
        }

        public override int SaveChanges()
        {
            var auditLogs = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var entityId = entry.Entity.Id;
                    var operation = entry.State.ToString();

                    var changes = entry.State == EntityState.Added
                        ? JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                        : entry.State == EntityState.Modified
                            ? JsonSerializer.Serialize(entry.Properties.Where(p => p.IsModified)
                                .ToDictionary(p => p.Metadata.Name, p => new
                                {
                                    OldValue = p.OriginalValue,
                                    NewValue = p.CurrentValue
                                }))
                            : null;

                    auditLogs.Add(new AuditLog
                    {
                        EntityName = entityName,
                        EntityId = entityId,
                        Operation = operation,
                        Changes = changes
                    });
                }
            }

            if (auditLogs.Any())
            {
                AuditLogs.AddRange(auditLogs);
            }

            return base.SaveChanges();
        }
    }
}