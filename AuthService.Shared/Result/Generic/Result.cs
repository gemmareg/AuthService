namespace AuthService.Shared.Result.Generic
{
    public class Result<T> : NonGeneric.Result
    {
        public T? Data { get; private set; }

        private Result(bool success, string message, T? data) : base(success, message)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static Result<T> Ok(T data)
        {
            return new Result<T>(true, string.Empty, data);
        }

        public static Result<T> Ok(string message, T data)
        {
            return new Result<T>(true, message, data);
        }

        public new static Result<T> Fail(string message)
        {
            return new Result<T>(false, message, default);
        }

        public override string ToString()
        {
            return Success ? $"✅ OK: {Message}" : $"❌ Error: {Message}";
        }

    }
}
