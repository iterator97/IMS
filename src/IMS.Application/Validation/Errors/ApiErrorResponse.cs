namespace IMS.Application.Validation.Errors
{
    public sealed record ApiErrorResponse
    {
        public required int StatusCode { get; init; }
        public required string Message { get; init; }
        public required string TraceId { get; init; }
    }
}
