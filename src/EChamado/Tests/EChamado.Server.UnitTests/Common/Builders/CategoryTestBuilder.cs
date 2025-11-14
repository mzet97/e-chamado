using AutoFixture;
using EChamado.Server.Domain.Domains.Orders.Entities;

namespace EChamado.Server.UnitTests.Common.Builders;

/// <summary>
/// Builder para criar instâncias de Category para testes
/// </summary>
public class CategoryTestBuilder
{
    private readonly Fixture _fixture;
    private string _name;
    private string _description;

    public CategoryTestBuilder()
    {
        _fixture = new Fixture();
        _name = _fixture.Create<string>();
        _description = _fixture.Create<string>();
    }

    public static CategoryTestBuilder Create() => new();

    public CategoryTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryTestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CategoryTestBuilder WithValidData()
    {
        return WithName("Test Category")
            .WithDescription("Test category description");
    }

    public CategoryTestBuilder WithEmptyName()
    {
        return WithName(string.Empty);
    }

    public CategoryTestBuilder WithEmptyDescription()
    {
        return WithDescription(string.Empty);
    }

    public CategoryTestBuilder WithLongName()
    {
        return WithName(new string('A', 256)); // Assumindo limite de 255 caracteres
    }

    public CategoryTestBuilder WithLongDescription()
    {
        return WithDescription(new string('A', 1001)); // Assumindo limite de 1000 caracteres
    }

    public Category Build()
    {
        return Category.Create(_name, _description);
    }
}