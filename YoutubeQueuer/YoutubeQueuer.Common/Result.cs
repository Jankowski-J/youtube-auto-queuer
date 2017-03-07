namespace YoutubeQueuer.Common
{
    public class Result
    {
        public bool IsSuccess { get; private set; }

        protected Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public static Result Succeed()
        {
            return new Result(true);
        }

        public static Result Fail()
        {
            return new Result(false);
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; private set; }

        protected Result(bool isSuccess, T data) : base(isSuccess)
        {
            Data = data;
        }

        protected Result(bool isSuccess) : base(isSuccess)
        {
        }

        public static Result<T> Succeed(T data)
        {
            return new Result<T>(true, data);
        }

        public new static Result<T> Fail()
        {
            return new Result<T>(false);
        }
    }
}