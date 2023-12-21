namespace Products_API.Helpers;

public class Result
{
    public string Error { get; private set; }
    public bool IsSuccess { get; private set; }

    private Result(){}

    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result Failure(string error)
    {
        return new Result
        {
            Error = error,
            IsSuccess = false
        };
    }
}