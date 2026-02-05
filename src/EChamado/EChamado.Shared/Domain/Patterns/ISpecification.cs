using System.Linq.Expressions;

namespace EChamado.Shared.Domain.Patterns;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}
