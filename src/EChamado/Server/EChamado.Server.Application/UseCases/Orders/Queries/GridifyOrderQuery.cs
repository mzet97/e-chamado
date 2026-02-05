using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Orders.ViewModels;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

/// <summary>
/// Query para busca de orders com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyOrderQuery : GridifySearchQuery<OrderViewModel>
{
    /// <summary>
    /// Filtro por ID da order
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por título da order
    /// Suporta operadores Gridify: title=*chamado*, title^=Urg, title$=teste
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Filtro por descrição da order
    /// Suporta operadores Gridify: description=*problema*, description^=Erro
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Filtro por ID do status
    /// Exemplo: statusId=123e4567-e89b-12d3-a456-426614174000
    /// </summary>
    public Guid? StatusId { get; set; }

    /// <summary>
    /// Filtro por ID do tipo de order
    /// </summary>
    public Guid? TypeId { get; set; }

    /// <summary>
    /// Filtro por ID da categoria
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Filtro por ID da subcategoria
    /// </summary>
    public Guid? SubCategoryId { get; set; }

    /// <summary>
    /// Filtro por ID do departamento
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Filtro por ID do usuário solicitante
    /// </summary>
    public Guid? RequestingUserId { get; set; }

    /// <summary>
    /// Filtro por email do usuário solicitante
    /// </summary>
    public string? RequestingUserEmail { get; set; }

    /// <summary>
    /// Filtro por ID do usuário responsável
    /// </summary>
    public Guid? ResponsibleUserId { get; set; }

    /// <summary>
    /// Filtro por email do usuário responsável
    /// </summary>
    public string? ResponsibleUserEmail { get; set; }

    /// <summary>
    /// Filtro por data de abertura
    /// Suporta operadores: openingDate>2024-01-01, openingDate<2024-12-31
    /// </summary>
    public DateTime? OpeningDate { get; set; }

    /// <summary>
    /// Filtro por data de fechamento
    /// </summary>
    public DateTime? ClosingDate { get; set; }

    /// <summary>
    /// Filtro por data de vencimento
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Filtro por data de criação
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Filtro por data de atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
