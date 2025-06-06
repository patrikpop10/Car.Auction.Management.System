namespace Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Problem? Problem { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Problem problem)
    {
        IsSuccess = false;
        Problem = problem;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Problem problem) => new(problem);
}

public class Result
{
    public bool IsSuccess { get; private init; }
    public string? ErrorCode { get; private init; }
    public string? ErrorMessage { get; private init; }

    public Problem? Problem { get; private init; }

    protected Result()
    {
        IsSuccess = true;
    }
    public static Result Success() => new();
    public static Result Failure(Problem problem) => new Result
    {
        IsSuccess = false,
        Problem = problem,
        ErrorCode = problem.Title,
        ErrorMessage = problem.ErrorMessage
    };
}