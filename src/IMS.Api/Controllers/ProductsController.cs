using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Helpers;
using IMS.Application.Products.Dto;
using IMS.Application.Products.Query;
using IMS.Application.Wrappes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ProductsController(ISender sender) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] QueryParams queryParams, CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new GetProducts.Query { QueryParams = queryParams },
                cancellationToken);

            return Ok(result);
        }
    }
}
