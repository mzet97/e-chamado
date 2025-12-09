namespace EChamado.Shared.Domain.Patterns;

public interface IAsyncFactory<T>
{
    Task<T> CreateAsync(CancellationToken ct);
}