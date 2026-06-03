using System;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Helpers;
using IMS.Application.Products.Command;
using IMS.Application.Products.Dto;
using IMS.Application.Products.Query;
using IMS.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ProductsController(ISender sender) : ControllerBase
    {
        private ActionResult<T> Handle<T>(Result<T> result)
        {
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            if (!result.IsSuccess && result.ErrorCode == StatusCodes.Status404NotFound)
                return NotFound();

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<PagedData<ProductDto>>> GetProducts([FromQuery] QueryParams queryParams, CancellationToken cancellationToken)
        {
            return Handle(await sender.Send(
                new GetProducts.Query { QueryParams = queryParams },
                cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductDto productDto, CancellationToken cancellationToken)
        {
            return Handle(await sender.Send(
                new CreateProduct.Command { ProductDto = productDto },
                cancellationToken));
        }
    }
}
