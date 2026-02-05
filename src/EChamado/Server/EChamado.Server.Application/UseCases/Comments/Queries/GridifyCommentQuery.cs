using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Comments.ViewModels;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

/// <summary>
/// Query para busca de comments com Gridify
/// Suporta filtros avançados, ordenação e paginação usando sintaxe Gridify
/// </summary>
public class GridifyCommentQuery : GridifySearchQuery<CommentViewModel>
{
    /// <summary>
    /// Filtro por ID do comment
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Filtro por texto do comment
    /// Suporta operadores Gridify: text=*problema*, text^=Olá, text$=adeus
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Filtro por ID da order
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// Filtro por ID do usuário
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filtro por email do usuário
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Filtro por data de criação
    /// Suporta operadores: createdAt>2024-01-01, createdAt<2024-12-31
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Filtro por data de atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Filtro por status de deleção
    /// </summary>
    public bool? IsDeleted { get; set; }
}
