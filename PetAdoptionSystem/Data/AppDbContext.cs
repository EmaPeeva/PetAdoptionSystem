using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Models; 

namespace PetAdoptionSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; } = null!;
        public DbSet<Adopter> Adopters { get; set; } = null!;
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<ApplicationUser>()
                .HasOne<Adopter>()
                .WithMany()
                .HasForeignKey(u => u.AdopterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
