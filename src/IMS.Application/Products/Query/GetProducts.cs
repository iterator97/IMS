using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Helpers;
using IMS.Application.Products.Dto;
using IMS.Application.Wrappes;
using IMS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace IMS.Application.Products.Query
{
    public class GetProducts
    {
        public class Query : IRequest<PagedResult<ProductDto>>
        {
            public required QueryParams QueryParams { get; init; }
        }

        public class Handler(AppDbContext context) : IRequestHandler<Query, PagedResult<ProductDto>>
        {
            public async Task<PagedResult<ProductDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var parameters = request.QueryParams;

                var pageNumber = parameters.PageNumber < 1 ? 1 : parameters.PageNumber;
                var pageSize = parameters.PageSize < 1 ? 20 : parameters.PageSize;

                var query = context.Products
                    .AsNoTracking()
                    .AsQueryable();

                query = parameters.SortBy switch
                {
                    "name" => parameters.SortDirection == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),
                    "price" => parameters.SortDirection == "desc"
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
                    _ => query.OrderBy(p => p.Name)
                };

                var totalCount = await query.CountAsync();


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

                return new PagedResult<ProductDto>(
                            products,
                            pageNumber,
                            pageSize,
                            totalCount);
            }
        }
    }
}
