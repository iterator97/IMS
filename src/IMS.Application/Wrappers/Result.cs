namespace IMS.Application.Wrappers
{
    public sealed record Result<T>
    {
        public T? Value;
        public bool IsSuccess;
        public bool IsError;
        public string? ErrorMessage;
        public int? ErrorCode;

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
