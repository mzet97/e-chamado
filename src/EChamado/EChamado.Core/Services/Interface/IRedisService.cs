namespace EChamado.Core.Services.Interface;

public interface IRedisService
{
    Task SetValueAsync(string key, string value);
    Task<string?> GetValueAsync(string key);
    Task RemoveValueAsync(string key);
}
