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
/// Controller OData para OrderTypes
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]

public class OrderTypesController(IOrderTypeRepository orderTypeRepository) : ODataController
{
    /// <summary>
    /// Obtém todos os order types com suporte a queries OData
    /// </summary>
    /// <returns>Lista de order types</returns>
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<OrderType> Get()
        => orderTypeRepository.GetAllQueryable();

    /// <summary>
    /// Obtém um order type por ID
    /// </summary>
    /// <param name="key">ID do order type</param>
    /// <returns>OrderType específico</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<OrderType> Get([FromODataUri] Guid key)
    {
        var result = orderTypeRepository.GetAllQueryable().Where(ot => ot.Id == key);
        return SingleResult.Create(result);
    }
}
