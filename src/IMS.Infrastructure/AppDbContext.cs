using IMS.Application.Shared;
using IMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(entity =>
            {
                entity.Property(product => product.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(product => product.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(product => product.Price)
                    .IsRequired();

                entity.Property(product => product.Stock)
                    .IsRequired();
            });
        }
    }
}
