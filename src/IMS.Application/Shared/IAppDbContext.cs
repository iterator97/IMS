using System.Threading;
using System.Threading.Tasks;
using IMS.Domain.Discounts;
using IMS.Domain.Orders;
using IMS.Domain.Products;
using IMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IMS.Application.Shared
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        DbSet<Address> Addresses { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Discount> Discounts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        DatabaseFacade Database { get; }
    }
}
