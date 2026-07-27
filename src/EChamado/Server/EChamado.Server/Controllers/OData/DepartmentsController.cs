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
/// Controller OData para Departments
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]

public class DepartmentsController(IDepartmentRepository departmentRepository) : ODataController
{
    /// <summary>
    /// Obtém todos os departments com suporte a queries OData
    /// </summary>
    /// <returns>Lista de departments</returns>
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<Department> Get()
        => departmentRepository.GetAllQueryable();

    /// <summary>
    /// Obtém um department por ID
    /// </summary>
    /// <param name="key">ID do department</param>
    /// <returns>Department específico</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<Department> Get([FromODataUri] Guid key)
    {
        var result = departmentRepository.GetAllQueryable().Where(d => d.Id == key);
        return SingleResult.Create(result);
    }
}
