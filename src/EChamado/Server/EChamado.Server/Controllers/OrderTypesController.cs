using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderTypesController : ControllerBase
{
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly ILogger<OrderTypesController> _logger;

    public OrderTypesController(
        IOrderTypeRepository orderTypeRepository,
        ILogger<OrderTypesController> logger)
    {
        _orderTypeRepository = orderTypeRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OrderTypeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderTypeResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var types = await _orderTypeRepository.GetAllAsync(cancellationToken);
            var response = types.Select(t => new OrderTypeResponse(t.Id, t.Name, t.Description)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order types");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderTypeResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var type = await _orderTypeRepository.GetByIdAsync(id, cancellationToken);
            if (type == null)
                return NotFound();

            return Ok(new OrderTypeResponse(type.Id, type.Name, type.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order type {TypeId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var type = OrderType.Create(request.Name, request.Description);
            await _orderTypeRepository.CreateAsync(type, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = type.Id }, type.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order type");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var type = await _orderTypeRepository.GetByIdAsync(id, cancellationToken);
            if (type == null)
                return NotFound();

            type.Update(request.Name, request.Description);
            await _orderTypeRepository.UpdateAsync(type, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order type {TypeId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var type = await _orderTypeRepository.GetByIdAsync(id, cancellationToken);
            if (type == null)
                return NotFound();

            await _orderTypeRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order type {TypeId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record OrderTypeResponse(Guid Id, string Name, string Description);
public record CreateOrderTypeRequest(string Name, string Description);
public record UpdateOrderTypeRequest(string Name, string Description);
