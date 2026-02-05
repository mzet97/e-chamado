using AutoFixture;
using AutoFixture.Kernel;
using System.Reflection;

namespace EChamado.Server.UnitTests.Common.Fixtures;

/// <summary>
/// Configuração customizada do AutoFixture para testes
/// </summary>
public class DomainAutoFixture : Fixture
{
    public DomainAutoFixture()
    {
        ConfigureCustomizations();
    }

    private void ConfigureCustomizations()
    {
        // Configurar GUIDs únicos
        this.Customize<Guid>(composer => composer.FromFactory(() => Guid.NewGuid()));

        // Configurar strings com tamanhos válidos
        this.Customize<string>(composer => composer.FromFactory(() =>
            this.Create<Generator<string>>().First().Substring(0, Math.Min(50, this.Create<Generator<string>>().First().Length))));

        // Configurar DateTime para valores válidos
        this.Customize<DateTime>(composer => composer.FromFactory(() =>
            DateTime.UtcNow.AddDays(this.Create<int>() % 365)));

        // Configurar propriedades específicas para evitar valores inválidos
        this.Customizations.Add(new PropertyOmitter("Id"));
        this.Customizations.Add(new PropertyOmitter("CreatedAt"));
        this.Customizations.Add(new PropertyOmitter("UpdatedAt"));
        this.Customizations.Add(new PropertyOmitter("DeletedAt"));
    }
}

/// <summary>
/// Customização para omitir propriedades específicas
/// </summary>
public class PropertyOmitter : ISpecimenBuilder
{
    private readonly string _propertyName;

    public PropertyOmitter(string propertyName)
    {
        _propertyName = propertyName;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property && property.Name == _propertyName)
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}