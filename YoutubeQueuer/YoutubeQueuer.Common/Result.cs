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
}