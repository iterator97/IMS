using System.Threading;
using System.Threading.Tasks;
using IMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Shared
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        DbSet<Address> Addresses { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
