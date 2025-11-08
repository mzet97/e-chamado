using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EChamado.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo chamado
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User not authenticated properly.");
            }

            var command = new CreateOrderCommand(
                request.Title,
                request.Description,
                request.TypeId,
                request.CategoryId,
                request.SubCategoryId,
                request.DepartmentId,
                request.DueDate,
                Guid.Parse(userId),
                userEmail
            );

            var orderId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = orderId }, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um chamado existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        try
        {
            var command = new UpdateOrderCommand(
                id,
                request.Title,
                request.Description,
                request.CategoryId,
                request.SubCategoryId,
                request.DepartmentId,
                request.DueDate
            );

            await _mediator.Send(command);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Order not found: {OrderId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Fecha um chamado
    /// </summary>
    [HttpPost("{id}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Close(Guid id, [FromBody] CloseOrderRequest request)
    {
        try
        {
            var command = new CloseOrderCommand(id, request.Evaluation);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error closing order: {OrderId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing order {OrderId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atribui um chamado a um usuário responsável
    /// </summary>
    [HttpPost("{id}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignOrderRequest request)
    {
        try
        {
            var command = new AssignOrderCommand(id, request.ResponsibleUserId, request.ResponsibleUserEmail);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Order not found: {OrderId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning order {OrderId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Busca um chamado por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderViewModel>> GetById(Guid id)
    {
        try
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { error = $"Order with ID {id} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Busca chamados com filtros e paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderListViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderListViewModel>>> Search([FromQuery] SearchOrdersQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching orders");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Busca chamados do usuário logado
    /// </summary>
    [HttpGet("my-tickets")]
    [ProducesResponseType(typeof(PagedResult<OrderListViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderListViewModel>>> GetMyTickets(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var query = new SearchOrdersQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                RequestingUserId: Guid.Parse(userId)
            );

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user tickets");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Busca chamados atribuídos ao usuário logado
    /// </summary>
    [HttpGet("assigned-to-me")]
    [ProducesResponseType(typeof(PagedResult<OrderListViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderListViewModel>>> GetAssignedToMe(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var query = new SearchOrdersQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                ResponsibleUserId: Guid.Parse(userId)
            );

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assigned tickets");
            return BadRequest(new { error = ex.Message });
        }
    }
}

// DTOs para requests
public record CreateOrderRequest(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);

public record UpdateOrderRequest(
    string Title,
    string Description,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);

public record CloseOrderRequest(int Evaluation);

public record AssignOrderRequest(Guid ResponsibleUserId, string ResponsibleUserEmail);
