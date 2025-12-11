namespace EChamado.Shared.Domain.Patterns;

public interface IFactory<T>
{
    T Create();
}