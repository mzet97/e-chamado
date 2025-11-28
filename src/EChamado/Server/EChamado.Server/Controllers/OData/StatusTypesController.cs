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
/// Controller OData para StatusTypes
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]

public class StatusTypesController(IStatusTypeRepository statusTypeRepository) : ODataController
{
    /// <summary>
    /// Obtém todos os status types com suporte a queries OData
    /// </summary>
    /// <returns>Lista de status types</returns>
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<StatusType> Get()
        => statusTypeRepository.GetAllQueryable();

    /// <summary>
    /// Obtém um status type por ID
    /// </summary>
    /// <param name="key">ID do status type</param>
    /// <returns>StatusType específico</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<StatusType> Get([FromODataUri] Guid key)
    {
        var result = statusTypeRepository.GetAllQueryable().Where(st => st.Id == key);
        return SingleResult.Create(result);
    }
}
