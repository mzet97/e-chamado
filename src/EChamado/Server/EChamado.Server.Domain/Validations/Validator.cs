using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Validations;

public static class Validator
{
    public static bool Validate<TV, TE>(TV validation, TE entity) where TV : AbstractValidator<TE> where TE : IEntity
    {
        var validator = validation.Validate(entity);

        if (validator.IsValid) return true;

        return false;
    }
}