using Xunit;

namespace EChamado.E2E.Tests.Common.Attributes;

public sealed class E2EFactAttribute : FactAttribute
{
    private const string ToggleVariable = "RUN_E2E_TESTS";

    public E2EFactAttribute()
    {
        var toggle = Environment.GetEnvironmentVariable(ToggleVariable);
        if (!string.Equals(toggle, "true", StringComparison.OrdinalIgnoreCase))
        {
            Skip = $"Set {ToggleVariable}=true to run Playwright end-to-end tests.";
        }
    }
}
