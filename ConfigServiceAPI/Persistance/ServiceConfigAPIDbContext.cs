using ConfigServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigServiceAPI.Persistance
{
    public class ServiceConfigAPIDbContext : DbContext
    {
        public ServiceConfigAPIDbContext(DbContextOptions<ServiceConfigAPIDbContext> options) : base(options) { }

        public DbSet<Enviroments> Enviroment { get; set; }
        public DbSet<Variables> Variables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enviroments>()
                .HasIndex(u => u.name)
                .IsUnique();

            modelBuilder.Entity<Variables>()
                .HasIndex(u => u.name)
                .IsUnique();

            modelBuilder.Entity<Variables>()
                .HasOne(v => v.Enviroment)
                .WithMany()
                .HasForeignKey(v => v.EnviromentId);


            base.OnModelCreating(modelBuilder);

        }
    }
}
