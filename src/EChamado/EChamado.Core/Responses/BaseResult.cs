using System.Text.Json.Serialization;

namespace EChamado.Core.Responses;

public class BaseResult
{
    public BaseResult(bool success = true, string message = "")
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; private set; }
    public string Message { get; private set; }
}

public class BaseResult<T> : BaseResult
{
    public BaseResult(IEnumerable<T> data, PagedResult pagedResult, bool success = true, string message = "") : base(success, message)
    {
        Data = data;
        PagedResult = pagedResult;
    }

    public IEnumerable<T> Data { get; private set; }
    public PagedResult PagedResult { get; private set; }
}