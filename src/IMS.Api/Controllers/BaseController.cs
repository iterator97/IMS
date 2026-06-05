using IMS.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers
{
    public abstract class BaseController(ISender sender) : ControllerBase
    {
        protected ISender Sender { get; init; } = sender;

        protected ActionResult<T> Handle<T>(Result<T> result)
        {
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            if (!result.IsSuccess && result.ErrorCode == StatusCodes.Status404NotFound)
                return NotFound(result.ErrorMessage);

            return BadRequest(result.ErrorMessage);
        }
    }
}
