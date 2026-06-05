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

            if (!result.IsSuccess)
            {
                var errorMessage = result.ErrorMessage ?? "Request failed.";

                return result.ErrorCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(errorMessage),
                    StatusCodes.Status400BadRequest or null => BadRequest(errorMessage),
                    _ => StatusCode(result.ErrorCode.Value, errorMessage)
                };
            }

            return BadRequest("Request failed.");
        }
    }
}
