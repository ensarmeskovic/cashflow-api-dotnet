using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cashflow.Domain;
using Cashflow.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cashflow.Infrastructure
{
    public class CashflowContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Inbox> Inboxes { get; set; }
        public virtual DbSet<InboxUser> InboxUsers { get; set; }

        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<ExpensePayment> ExpensePayments { get; set; }



        public CashflowContext(DbContextOptions options) : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries();
            foreach (EntityEntry entry in entries)
            {
                if (!(entry.Entity is IEntity entity))
                    continue;

                DateTime now = DateTime.Now;

                switch (entry.State)
                {
                    case EntityState.Added: { entity.AddedDateTime = now; } break;
                    case EntityState.Modified: { entity.ModifiedDateTime = entity.DeletedDateTime.HasValue ? entity.ModifiedDateTime : now; } break; //soft and hard delete
                    case EntityState.Deleted: { entity.DeletedDateTime = now; } break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Remove all cascade relationships */
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).ToList().ForEach(r => r.DeleteBehavior = DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
