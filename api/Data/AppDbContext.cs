using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<GymPackage> GymPackages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<CheckInHistory> CheckInHistories { get; set; }
        public DbSet<BodyIndex> BodyIndices { get; set; }
    }
}