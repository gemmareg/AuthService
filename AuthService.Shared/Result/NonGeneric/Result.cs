namespace AuthService.Shared.Result.NonGeneric
{
    public class Result
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        protected Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result Ok(string message)
        {
            return new Result(true, message);
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public override string ToString()
        {
            return Success ? $"✅ OK: {Message}" : $"❌ Error: {Message}";
        }
    }
}
