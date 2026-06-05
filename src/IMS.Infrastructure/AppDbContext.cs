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
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Discount> Discounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(entity =>
            {
                entity.ToTable(table => table.HasCheckConstraint(
                    "CK_Product_Stock_NonNegative",
                    "\"Stock\" >= 0"));

                entity.Property(product => product.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(product => product.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(product => product.Price)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(product => product.Stock)
                    .IsRequired();
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(user => user.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(user => user.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(user => user.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(user => user.Addresses)
                    .WithOne(address => address.User)
                    .HasForeignKey(address => address.UserId);

                entity.HasMany(user => user.Orders)
                    .WithOne(order => order.User)
                    .HasForeignKey(order => order.UserId);
            });

            builder.Entity<Address>(entity =>
            {
                entity.Property(address => address.Street)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(address => address.Number)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(address => address.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(address => address.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(address => address.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(address => address.Continent)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            builder.Entity<Order>(entity =>
            {
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint(
                        "CK_Orders_LocationCharge_Range",
                        "\"LocationCharge\" >= 0 AND \"LocationCharge\" <= 1");

                    table.HasCheckConstraint(
                        "CK_Orders_TotalAmount_NonNegative",
                        "\"TotalAmount\" >= 0");
                });

                entity.Property(order => order.CreatedAt)
                    .IsRequired();

                entity.Property(order => order.LocationCharge)
                    .HasPrecision(5, 4)
                    .IsRequired();

                entity.Property(order => order.TotalAmount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.HasOne(order => order.Address)
                    .WithMany()
                    .HasForeignKey(order => order.AddressId);

                entity.HasMany(order => order.Items)
                    .WithOne(item => item.Order)
                    .HasForeignKey(item => item.OrderId);
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(item => item.Quantity)
                    .IsRequired();

                entity.Property(item => item.Discount)
                    .HasPrecision(5, 4)
                    .IsRequired();

                entity.HasOne(item => item.Product)
                    .WithMany()
                    .HasForeignKey(item => item.ProductId);
            });

            builder.Entity<Discount>(entity =>
            {
                entity.Property(discount => discount.StartDate)
                    .IsRequired();

                entity.Property(discount => discount.EndDate)
                    .IsRequired();

                entity.Property(discount => discount.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(discount => discount.Continent)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(discount => discount.Amount)
                    .HasPrecision(5, 4)
                    .IsRequired();

                entity.Property(discount => discount.Enabled)
                    .IsRequired();

                entity.Property(discount => discount.Mode)
                    .IsRequired();
            });
        }
    }
}
