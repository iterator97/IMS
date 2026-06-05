using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace IMS.Application.Validation
{
    public sealed class PipelineValidationBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var validationResults = await Task.WhenAll(
                validators.Select(validator =>
                    validator.ValidateAsync(request, cancellationToken)));

            var errors = validationResults
                .SelectMany(result => result.Errors)
                .ToArray();

            if (errors.Length > 0)
                throw new ValidationException(errors);

            return await next(cancellationToken);
        }
    }
}
