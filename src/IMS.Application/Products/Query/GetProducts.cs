using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Helpers;
using IMS.Application.Products.Dto;
using IMS.Application.Shared;
using IMS.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Products.Query
{
    public class GetProducts
    {
        public sealed record Query : IRequest<Result<PagedData<ProductDto>>>
        {
            public required QueryParams QueryParams { get; init; }
        }

        public sealed class Handler(IAppDbContext context) : IRequestHandler<Query, Result<PagedData<ProductDto>>>
        {
            public async Task<Result<PagedData<ProductDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var parameters = request.QueryParams;

                var pageNumber = parameters.PageNumber < 1 ? 1 : parameters.PageNumber;
                var pageSize = parameters.PageSize < 1 ? 20 : parameters.PageSize;
                var sortBy = parameters.SortBy?.Trim().ToLowerInvariant() ?? "name";
                var sortDirection = parameters.SortDirection?.Trim().ToLowerInvariant() ?? "asc";

                var query = context.Products
                    .AsNoTracking()
                    .AsQueryable();

                query = sortBy switch
                {
                    "name" => sortDirection == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),
                    "price" => sortDirection == "desc"
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
                    _ => query.OrderBy(p => p.Name)
                };

                var totalCount = await query.CountAsync(cancellationToken);

                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Stock = p.Stock
                    })
                    .ToListAsync(cancellationToken);

                return Result<PagedData<ProductDto>>.Success(new PagedData<ProductDto>(
                        products,
                        pageNumber,
                        pageSize,
                        totalCount));
            }
        }
    }
}
