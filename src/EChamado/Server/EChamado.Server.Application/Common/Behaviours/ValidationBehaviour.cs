using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace EChamado.Server.Application.Common.Behaviours;

/// <summary>
/// Attribute to mark handlers that should have validation.
/// </summary>
public class RequestValidationAttribute : RequestHandlerAttribute
{
    public RequestValidationAttribute(int step, HandlerTiming timing = HandlerTiming.Before)
        : base(step, timing)
    {
    }

    public override Type GetHandlerType()
    {
        return typeof(ValidationHandler<>);
    }
}

/// <summary>
/// Validation handler for Brighter pipeline.
/// Runs FluentValidation validators before the main handler executes.
/// </summary>
public class ValidationHandler<TRequest> : RequestHandlerAsync<TRequest>
    where TRequest : class, IRequest
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var validators = _serviceProvider.GetServices<IValidator<TRequest>>();

        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(command);

            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

