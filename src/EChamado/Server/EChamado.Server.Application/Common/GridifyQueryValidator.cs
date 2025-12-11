using FluentValidation;

namespace EChamado.Server.Application.Common;

/// <summary>
/// Validador base para queries Gridify
/// Valida parâmetros de paginação e filtragem
/// </summary>
/// <typeparam name="TQuery">Tipo da query</typeparam>
/// <typeparam name="TResult">Tipo do resultado</typeparam>
public class GridifyQueryValidator<TQuery, TResult> : AbstractValidator<TQuery>
    where TQuery : GridifySearchQuery<TResult>
{
    public GridifyQueryValidator()
    {
        // Validações de paginação
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page deve ser maior que 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Page não pode ser maior que 10000");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize deve ser maior que 0")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize não pode ser maior que 100 para evitar sobrecarga");

        // Validação de filter length (evitar filtros muito grandes que possam causar problemas)
        RuleFor(x => x.Filter)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Filter))
            .WithMessage("Filter não pode ter mais de 500 caracteres");

        // Validação de orderBy length
        RuleFor(x => x.OrderBy)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.OrderBy))
            .WithMessage("OrderBy não pode ter mais de 200 caracteres");
    }
}
