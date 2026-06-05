using System;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Products.Command;
using IMS.Application.Products.Dto;
using IMS.Application.Products.Query;
using IMS.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ProductsController(ISender sender) : BaseController(sender)
    {
        [HttpGet]
        public async Task<ActionResult<PagedData<ProductDto>>> GetProducts([FromQuery] QueryParams queryParams, CancellationToken cancellationToken)
        {
            return Handle(await Sender.Send(
                new GetProducts.Query { QueryParams = queryParams },
                cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductDto productDto, CancellationToken cancellationToken)
        {
            return Handle(await Sender.Send(
                new CreateProduct.Command { ProductDto = productDto },
                cancellationToken));
        }
    }
}
