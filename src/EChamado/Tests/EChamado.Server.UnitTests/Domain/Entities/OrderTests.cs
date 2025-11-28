using EChamado.Server.Domain.Domains.Orders;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class OrderTests : UnitTestBase
{
    private static readonly IDateTimeProvider _dateTimeProvider = new SystemDateTimeProvider();

    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var title = "Chamado de Teste";
        var description = "Descri��o do chamado";
        var requestingUserEmail = "user@test.com";
        var responsibleUserEmail = "admin@test.com";
        var requestingUserId = Guid.NewGuid();
        var responsibleUserId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();
        var statusTypeId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(30);

        // Act
        var order = Order.Create(
            title, description, requestingUserEmail, responsibleUserEmail,
            requestingUserId, responsibleUserId, categoryId, departmentId,
            orderTypeId, statusTypeId, subCategoryId, dueDate, _dateTimeProvider);

        // Assert
        order.Should().NotBeNull();
        order.Title.Should().Be(title);
        order.Description.Should().Be(description);
        order.RequestingUserEmail.Should().Be(requestingUserEmail);
        order.ResponsibleUserEmail.Should().Be(responsibleUserEmail); // Corrigido - agora usa o valor correto
        order.RequestingUserId.Should().Be(requestingUserId);
        order.ResponsibleUserId.Should().Be(responsibleUserId);
        order.CategoryId.Should().Be(categoryId);
        order.DepartmentId.Should().Be(departmentId);
        order.TypeId.Should().Be(orderTypeId);
        order.StatusId.Should().Be(statusTypeId);
        order.SubCategoryId.Should().Be(subCategoryId);
        order.DueDate.Should().Be(dueDate);
        order.Id.Should().NotBe(Guid.Empty);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        order.OpeningDate.Should().NotBeNull(); // Corrigido - OpeningDate agora � definida
        order.OpeningDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        order.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidOrder()
    {
        // Arrange
        var title = ""; // T�tulo inv�lido
        var description = "Descri��o v�lida";
        var requestingUserEmail = "user@test.com";
        var responsibleUserEmail = "admin@test.com";
        var requestingUserId = Guid.NewGuid();
        var responsibleUserId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();
        var statusTypeId = Guid.NewGuid();

        // Act
        var order = Order.Create(
            title, description, requestingUserEmail, responsibleUserEmail,
            requestingUserId, responsibleUserId, categoryId, departmentId,
            orderTypeId, statusTypeId, null, null, _dateTimeProvider);

        // Assert
        order.Should().NotBeNull();
        order.IsValid().Should().BeFalse();
        order.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();

        // Act & Assert
        order.Id.Should().NotBe(Guid.Empty);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        order.UpdatedAt.Should().BeNull();
        order.DeletedAt.Should().BeNull();
        order.IsDeleted.Should().BeFalse();
        order.OpeningDate.Should().NotBeNull(); // Corrigido - OpeningDate agora � definida
        order.OpeningDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)); // Validar que est� pr�xima do momento atual
        order.ClosingDate.Should().BeNull();
        order.Evaluation.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateOrder()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();
        var newTitle = "T�tulo Atualizado";
        var newDescription = "Descri��o Atualizada";
        var newRequestingUserEmail = "newuser@test.com";
        var newRequestingUserId = Guid.NewGuid();
        var newResponsibleUserId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var newDepartmentId = Guid.NewGuid();
        var newOrderTypeId = Guid.NewGuid();
        var newStatusTypeId = Guid.NewGuid();
        var newSubCategoryId = Guid.NewGuid();
        var newDueDate = DateTime.UtcNow.AddDays(60);

        // Act
        order.Update(
            newTitle, newDescription, newRequestingUserEmail,
            newRequestingUserId, newResponsibleUserId, newCategoryId,
            newDepartmentId, newOrderTypeId, newStatusTypeId,
            newSubCategoryId, newDueDate, _dateTimeProvider);

        // Assert
        order.Title.Should().Be(newTitle);
        order.Description.Should().Be(newDescription);
        order.RequestingUserEmail.Should().Be(newRequestingUserEmail);
        order.RequestingUserId.Should().Be(newRequestingUserId);
        order.ResponsibleUserId.Should().Be(newResponsibleUserId);
        order.CategoryId.Should().Be(newCategoryId);
        order.DepartmentId.Should().Be(newDepartmentId);
        order.TypeId.Should().Be(newOrderTypeId);
        order.StatusId.Should().Be(newStatusTypeId);
        order.SubCategoryId.Should().Be(newSubCategoryId);
        order.DueDate.Should().Be(newDueDate);
        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AssignTo_WithValidData_ShouldAssignOrder()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();
        var newResponsibleUserId = Guid.NewGuid();
        var newResponsibleUserEmail = "newresponsible@test.com";

        // Act
        order.AssignTo(newResponsibleUserId, newResponsibleUserEmail, _dateTimeProvider);

        // Assert
        order.ResponsibleUserId.Should().Be(newResponsibleUserId);
        order.ResponsibleUserEmail.Should().Be(newResponsibleUserEmail);
        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ChangeStatus_WithValidStatusId_ShouldChangeStatus()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();
        var newStatusId = Guid.NewGuid();

        // Act
        order.ChangeStatus(newStatusId, _dateTimeProvider);

        // Assert
        order.StatusId.Should().Be(newStatusId);
        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Close_WithValidEvaluation_ShouldCloseOrder(int evaluation)
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();

        // Act
        order.Close(evaluation, _dateTimeProvider);

        // Assert
        order.Evaluation.Should().Be(evaluation.ToString());
        order.ClosingDate.Should().NotBeNull();
        order.ClosingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldWork()
    {
        // Arrange & Act
        var order = Order.Create(
            "T�tulo", "Descri��o", "user@test.com", "admin@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, null, _dateTimeProvider); // SubCategory e DueDate opcionais

        // Assert
        order.Should().NotBeNull();
        order.SubCategoryId.Should().BeNull();
        order.DueDate.Should().BeNull();
        order.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithSubCategoryAndDueDate_ShouldSetValues()
    {
        // Arrange
        var subCategoryId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(15);

        // Act
        var order = Order.Create(
            "T�tulo", "Descri��o", "user@test.com", "admin@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), subCategoryId, dueDate, _dateTimeProvider);

        // Assert
        order.SubCategoryId.Should().Be(subCategoryId);
        order.DueDate.Should().Be(dueDate);
    }

    [Theory]
    [InlineData("T�tulo Simples")]
    [InlineData("T�tulo com acentos: ��o, �, �")]
    [InlineData("Title with numbers 123")]
    [InlineData("T�TULO MAI�SCULO")]
    public void Create_WithDifferentValidTitles_ShouldWork(string title)
    {
        // Act
        var order = Order.Create(
            title, "Descri��o", "user@test.com", "admin@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, null, _dateTimeProvider);

        // Assert
        order.Title.Should().Be(title);
        order.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Order_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();

        // Act & Assert
        var titleProperty = typeof(Order).GetProperty(nameof(Order.Title));
        var descriptionProperty = typeof(Order).GetProperty(nameof(Order.Description));

        titleProperty!.SetMethod.Should().NotBeNull();
        titleProperty.SetMethod!.IsPublic.Should().BeFalse();

        descriptionProperty!.SetMethod.Should().NotBeNull();
        descriptionProperty.SetMethod!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_MultipleTimes_ShouldHaveUniqueIds()
    {
        // Act
        var order1 = OrderTestBuilder.Create().WithValidData().Build();
        var order2 = OrderTestBuilder.Create().WithValidData().Build();

        // Assert
        order1.Id.Should().NotBe(order2.Id);
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();

        // Act
        order.Update("T�tulo 1", "Descri��o 1", "user1@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, null, _dateTimeProvider);
        var firstUpdate = order.UpdatedAt;

        Thread.Sleep(100);

        order.Update("T�tulo 2", "Descri��o 2", "user2@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, null, _dateTimeProvider);
        var secondUpdate = order.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        order.Title.Should().Be("T�tulo 2");
    }

    [Fact]
    public void Create_WithBoundaryValues_ShouldWork()
    {
        // Arrange
        var maxTitle = new string('A', 200);
        var maxDescription = new string('B', 500);

        // Act
        var order = Order.Create(
            maxTitle, maxDescription, "user@test.com", "admin@test.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, null, _dateTimeProvider);

        // Assert
        order.Title.Should().Be(maxTitle);
        order.Description.Should().Be(maxDescription);
        order.IsValid().Should().BeTrue();
    }
}