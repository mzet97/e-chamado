using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace EChamado.Server.Controllers.OData;

/// <summary>
/// Controller OData para Categories
/// </summary>
[Authorize]
[Route("odata/[controller]")]
public class CategoriesController(ICategoryRepository categoryRepository) : ODataController
{
    /// <summary>
    /// Obtém todas as categories com suporte a queries OData
    /// </summary>
    /// <returns>Lista de categories</returns>
    [HttpGet]
    [HttpGet("$count")]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<Category> Get()
        => categoryRepository.GetAllQueryable();

    /// <summary>
    /// Obtém uma category por ID
    /// </summary>
    /// <param name="key">ID da category</param>
    /// <returns>Category específica</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<Category> Get([FromODataUri] Guid key)
    {
        var result = categoryRepository.GetAllQueryable().Where(c => c.Id == key);
        return SingleResult.Create(result);
    }
}
