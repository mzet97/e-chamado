using EChamado.Server.Domain.Domains.Categories;
using EChamado.Server.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ILogger<CategoriesController> logger)
    {
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            var subCategories = await _subCategoryRepository.GetAllAsync(cancellationToken);

            var response = categories.Select(c => new CategoryResponse(
                c.Id,
                c.Name,
                c.Description,
                subCategories.Where(sc => sc.CategoryId == c.Id)
                    .Select(sc => new SubCategoryResponse(sc.Id, sc.Name, sc.Description, sc.CategoryId))
                    .ToList()
            )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return NotFound();

            var subCategories = await _subCategoryRepository.SearchAsync(
                sc => sc.CategoryId == id,
                cancellationToken);

            var response = new CategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                subCategories.Select(sc => new SubCategoryResponse(sc.Id, sc.Name, sc.Description, sc.CategoryId)).ToList()
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var category = Category.Create(request.Name, request.Description);
            await _categoryRepository.CreateAsync(category, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return NotFound();

            category.Update(request.Name, request.Description);
            await _categoryRepository.UpdateAsync(category, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
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
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return NotFound();

            await _categoryRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    // SubCategories endpoints
    [HttpPost("{categoryId}/subcategories")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> CreateSubCategory(
        Guid categoryId,
        [FromBody] CreateSubCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                return NotFound($"Category {categoryId} not found");

            var subCategory = SubCategory.Create(request.Name, request.Description, categoryId);
            await _subCategoryRepository.CreateAsync(subCategory, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = categoryId }, subCategory.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subcategory");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("subcategories/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubCategory(
        Guid id,
        [FromBody] UpdateSubCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (subCategory == null)
                return NotFound();

            subCategory.Update(request.Name, request.Description);
            await _subCategoryRepository.UpdateAsync(subCategory, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subcategory {SubCategoryId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("subcategories/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (subCategory == null)
                return NotFound();

            await _subCategoryRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subcategory {SubCategoryId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

// DTOs
public record CategoryResponse(Guid Id, string Name, string Description, List<SubCategoryResponse> SubCategories);
public record SubCategoryResponse(Guid Id, string Name, string Description, Guid CategoryId);
public record CreateCategoryRequest(string Name, string Description);
public record UpdateCategoryRequest(string Name, string Description);
public record CreateSubCategoryRequest(string Name, string Description);
public record UpdateSubCategoryRequest(string Name, string Description);
