namespace ApiAggregator.Shared
{
    public class Result<T> : Result
    {
        public T? Data { get; init; }

        public Result(bool isSuccess, T? data, Errors.Error? error)
            : base(isSuccess, error)
        {
            Data = data;
        }
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, Errors.Error.None);
        }

        public static new Result<T> Failure(Errors.Error error)
        {
            return new Result<T>(false, default, error);
        }
    }

}
