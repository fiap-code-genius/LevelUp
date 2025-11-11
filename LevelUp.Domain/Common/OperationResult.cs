namespace LevelUp.Domain.Common
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = 200;
        public T Value { get; set; }

        private OperationResult()
        {      
        }

        public static OperationResult<T> Success(T value, int statusCode = 200)
        {
            return new OperationResult<T> { IsSuccess = true, StatusCode = statusCode, Value = value };
        }
        public static OperationResult<T> Failure(string error, int statusCode = 400)
        {
            return new OperationResult<T> { IsSuccess = false, StatusCode = statusCode, Error = error };
        }
    }
}
