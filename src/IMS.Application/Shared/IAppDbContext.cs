using System.Threading;
using System.Threading.Tasks;
using IMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Shared
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
