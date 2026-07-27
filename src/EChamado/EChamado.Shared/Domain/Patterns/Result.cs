namespace EChamado.Shared.Domain.Patterns;

public record Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public string ErrorMessage => string.Join("; ", Errors);

    protected Result() { }
    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(params string[] errors) => new() { Errors = errors };
    public static Result Failure(IEnumerable<string> errors) => new() { Errors = errors };
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(params string[] errors) => Result<T>.Failure(errors);
}

public record Result<T> : Result
{
    public T? Value { get; init; }

    private Result() : base() { }
    private Result(T value) : base(true, Array.Empty<string>()) { Value = value; }
    private Result(IEnumerable<string> errors) : base(false, errors) { Value = default; }

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(params string[] errors) => new(errors);
    public static new Result<T> Failure(IEnumerable<string> errors) => new(errors);

    public static implicit operator Result<T>(T value) => Success(value);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IEnumerable<string>, TResult> onFailure)
        => IsSuccess && Value != null ? onSuccess(Value) : onFailure(Errors);
}
