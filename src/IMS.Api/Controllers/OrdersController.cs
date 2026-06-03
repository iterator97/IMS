using System;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Orders.Command;
using IMS.Application.Orders.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class OrdersController(ISender sender) : BaseController(sender)
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderDto orderDto, CancellationToken cancellationToken)
        {
            return Handle(await Sender.Send(
                new CreateOrder.Command { Order = orderDto },
                cancellationToken));
        }
    }
}
