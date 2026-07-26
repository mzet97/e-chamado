using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

/// <summary>
/// Política de SLA por categoria/departamento.
/// Define tempos de resposta e resolução esperados.
/// </summary>
public class SlaPolicy : SoftDeletableEntity<SlaPolicy>
{
    public string Name { get; private set; } = string.Empty;
    public Guid? CategoryId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public int ResponseHours { get; private set; }
    public int ResolutionHours { get; private set; }
    public int Priority { get; private set; } // 1=Alta, 2=Média, 3=Baixa

    // Navigation
    public Category? Category { get; private set; }
    public Department? Department { get; private set; }

    private SlaPolicy() : base(null!) { }

    public static SlaPolicy Create(
        string name,
        Guid? categoryId,
        Guid? departmentId,
        int responseHours,
        int resolutionHours,
        int priority,
        IDateTimeProvider dateTimeProvider)
    {
        var policy = new SlaPolicy
        {
            Id = Guid.NewGuid(),
            Name = name,
            CategoryId = categoryId,
            DepartmentId = departmentId,
            ResponseHours = responseHours,
            ResolutionHours = resolutionHours,
            Priority = priority
        };
        policy.MarkCreated(dateTimeProvider.UtcNow);
        return policy;
    }

    public override void Validate()
    {
        _isValid = !string.IsNullOrWhiteSpace(Name) &&
                   ResponseHours > 0 &&
                   ResolutionHours > 0 &&
                   Priority >= 1 && Priority <= 3;
        _errors = _isValid ? Enumerable.Empty<string>() : new[] { "SlaPolicy validation failed" };
    }
}
