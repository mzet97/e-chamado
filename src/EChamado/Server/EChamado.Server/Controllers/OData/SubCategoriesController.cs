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
/// Controller OData para SubCategories
/// </summary>
[Authorize]
[Route("odata/[controller]")]
public class SubCategoriesController(ISubCategoryRepository subCategoryRepository) : ODataController
{
    /// <summary>
    /// Obtém todas as subcategories com suporte a queries OData
    /// </summary>
    /// <returns>Lista de subcategories</returns>
    [HttpGet]
    [HttpGet("$count")]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<SubCategory> Get()
        => subCategoryRepository.GetAllQueryable();

    /// <summary>
    /// Obtém uma subcategory por ID
    /// </summary>
    /// <param name="key">ID da subcategory</param>
    /// <returns>SubCategory específica</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<SubCategory> Get([FromODataUri] Guid key)
    {
        var result = subCategoryRepository.GetAllQueryable().Where(sc => sc.Id == key);
        return SingleResult.Create(result);
    }
}
