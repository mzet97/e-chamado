using System.ComponentModel.DataAnnotations;
using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Comments.DTOs;

/// <summary>
/// DTO para criação de comentário
/// </summary>
public class CreateCommentRequestDto
{
    /// <summary>
    /// Texto do comentário
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(1000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário que está comentando
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Email do usuário que está comentando
    /// </summary>
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string? UserEmail { get; set; }
}

/// <summary>
/// DTO para parâmetros de busca de comentários por orderId
/// </summary>
public class GetCommentsByOrderIdParametersDto
{
    /// <summary>
    /// ID do chamado para buscar comentários
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid OrderId { get; set; }
}