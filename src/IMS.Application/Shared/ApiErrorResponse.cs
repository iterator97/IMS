namespace IMS.Application.Shared
{
    public sealed record ApiErrorResponse
    {
        public required int StatusCode { get; init; }
        public required string Message { get; init; }
        public string? TraceId { get; init; }
    }
}
