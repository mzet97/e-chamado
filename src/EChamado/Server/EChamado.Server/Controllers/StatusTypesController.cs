using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatusTypesController : ControllerBase
{
    private readonly IStatusTypeRepository _statusTypeRepository;
    private readonly ILogger<StatusTypesController> _logger;

    public StatusTypesController(
        IStatusTypeRepository statusTypeRepository,
        ILogger<StatusTypesController> logger)
    {
        _statusTypeRepository = statusTypeRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<StatusTypeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StatusTypeResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var statuses = await _statusTypeRepository.GetAllAsync(cancellationToken);
            var response = statuses.Select(s => new StatusTypeResponse(s.Id, s.Name, s.Description)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting status types");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StatusTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StatusTypeResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var status = await _statusTypeRepository.GetByIdAsync(id, cancellationToken);
            if (status == null)
                return NotFound();

            return Ok(new StatusTypeResponse(status.Id, status.Name, status.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting status type {StatusId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateStatusTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var status = StatusType.Create(request.Name, request.Description);
            await _statusTypeRepository.CreateAsync(status, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = status.Id }, status.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating status type");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStatusTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var status = await _statusTypeRepository.GetByIdAsync(id, cancellationToken);
            if (status == null)
                return NotFound();

            status.Update(request.Name, request.Description);
            await _statusTypeRepository.UpdateAsync(status, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status type {StatusId}", id);
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
            var status = await _statusTypeRepository.GetByIdAsync(id, cancellationToken);
            if (status == null)
                return NotFound();

            await _statusTypeRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting status type {StatusId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record StatusTypeResponse(Guid Id, string Name, string Description);
public record CreateStatusTypeRequest(string Name, string Description);
public record UpdateStatusTypeRequest(string Name, string Description);
