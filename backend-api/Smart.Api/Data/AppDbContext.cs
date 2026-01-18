using Microsoft.EntityFrameworkCore;
using Smart.Api.Models;

namespace Smart.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RfidCard> RfidCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.RfidCard)
                .WithOne(c => c.User)
                .HasForeignKey<RfidCard>(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RfidCard>()
                .HasIndex(c => c.CardUid)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
        }
    }
}
