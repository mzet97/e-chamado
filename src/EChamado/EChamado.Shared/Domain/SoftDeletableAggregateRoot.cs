using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EChamado.Shared.Domain;

public abstract class SoftDeletableAggregateRoot<T> : SoftDeletableEntity<T>
    where T : SoftDeletableAggregateRoot<T>
{
    protected SoftDeletableAggregateRoot(IValidator<T> validator) : base(validator) { }
    protected SoftDeletableAggregateRoot(IValidator<T> validator, Guid id, DateTime createdAtUtc)
        : base(validator, id, createdAtUtc) { }
}
