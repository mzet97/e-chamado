using EChamado.Server.Domain.Domains.Departments;
using EChamado.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(
        IDepartmentRepository departmentRepository,
        ILogger<DepartmentsController> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DepartmentResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var response = departments.Select(d => new DepartmentResponse(d.Id, d.Name, d.Description)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return NotFound();

            return Ok(new DepartmentResponse(department.Id, department.Name, department.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {DepartmentId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var department = Department.Create(request.Name, request.Description);
            await _departmentRepository.CreateAsync(department, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return NotFound();

            department.Update(request.Name, request.Description);
            await _departmentRepository.UpdateAsync(department, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department {DepartmentId}", id);
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
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return NotFound();

            await _departmentRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record DepartmentResponse(Guid Id, string Name, string Description);
public record CreateDepartmentRequest(string Name, string Description);
public record UpdateDepartmentRequest(string Name, string Description);
