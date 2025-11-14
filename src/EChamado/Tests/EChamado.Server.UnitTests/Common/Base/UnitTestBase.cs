using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using EChamado.Server.UnitTests.Common.Fixtures;

namespace EChamado.Server.UnitTests.Common.Base;

/// <summary>
/// Classe base para todos os testes unitários
/// </summary>
public abstract class UnitTestBase : IDisposable
{
    protected readonly DomainAutoFixture Fixture;
    protected readonly CancellationToken CancellationToken;

    protected UnitTestBase()
    {
        Fixture = new DomainAutoFixture();
        CancellationToken = CancellationToken.None;
    }

    /// <summary>
    /// Cria uma lista com número específico de itens
    /// </summary>
    protected List<T> CreateMany<T>(int count) => Fixture.CreateMany<T>(count).ToList();

    /// <summary>
    /// Cria um item único
    /// </summary>
    protected T Create<T>() => Fixture.Create<T>();

    /// <summary>
    /// Cria um item com customizações
    /// </summary>
    protected T Create<T>(Action<IPostprocessComposer<T>> customize)
    {
        var composer = Fixture.Build<T>();
        customize(composer);
        return composer.Create();
    }

    public virtual void Dispose()
    {
        // Implementar limpeza se necessário
        GC.SuppressFinalize(this);
    }
}