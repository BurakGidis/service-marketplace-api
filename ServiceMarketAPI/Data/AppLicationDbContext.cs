using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ServiceListing> ServiceListings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<ServiceListing>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Temizlik" },
                new Category { Id = 2, Name = "Bakıcı" },
                new Category { Id = 3, Name = "Şoför" },
                new Category { Id = 4, Name = "Boyacı" },
                new Category { Id = 5, Name = "Aşçı" },
                new Category { Id = 6, Name = "Garson (Vasıfsız Eleman)" },
                new Category { Id = 7, Name = "Elektrikçi" },
                new Category { Id = 8, Name = "Doğalgaz - Su Tesisatçı" },
                new Category { Id = 9, Name = "Halı Yıkamacı" },
                new Category { Id = 10, Name = "Evcil Hayvan Gezdiriciliği" }
            );
        }
    }
}