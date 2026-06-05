namespace IMS.Application.Wrappers
{
    public sealed record Result<T>
    {
        public T? Value { get; init; }
        public bool IsSuccess { get; init; }
        public bool IsError { get; init; }
        public string? ErrorMessage { get; init; }
        public int? ErrorCode { get; init; }

        public static Result<T> Success(T value) => new()
        {
            Value = value,
            IsSuccess = true,
            IsError = false
        };

        public static Result<T> Error(string errorMessage, int? errorCode = null) => new()
        {
            IsSuccess = false,
            IsError = true,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
}
