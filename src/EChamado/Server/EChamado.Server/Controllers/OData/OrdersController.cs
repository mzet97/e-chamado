using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace EChamado.Server.Controllers.OData;

/// <summary>
/// Controller OData para Orders (Chamados)
/// </summary>
/// <remarks>
/// Suporta queries OData padrão: $filter, $orderby, $select, $expand, $top, $skip, $count
/// </remarks>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class OrdersController(IOrderRepository orderRepository) : ODataController
{
    /// <summary>
    /// Obtém todas as orders com suporte a queries OData
    /// </summary>
    /// <remarks>
    /// Exemplo de uso:
    /// - GET odata/Orders?$filter=StatusId eq guid'...'
    /// - GET odata/Orders?$orderby=CreatedAt desc&amp;$top=50
    /// - GET odata/Orders?$expand=Status,Type,Category
    /// - GET odata/Orders/$count
    /// </remarks>
    /// <returns>Lista de orders com suporte a queries OData</returns>
    /// <response code="200">Retorna a lista de orders</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 5)]
    [ProducesResponseType(typeof(IQueryable<Order>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IQueryable<Order> Get()
        => orderRepository.GetAllQueryable();

    /// <summary>
    /// Obtém uma order específica por ID
    /// </summary>
    /// <param name="key">ID da order (GUID)</param>
    /// <remarks>
    /// Exemplo: GET odata/Orders(guid'12345678-1234-1234-1234-123456789012')
    /// </remarks>
    /// <returns>Order específica ou 404 se não encontrada</returns>
    /// <response code="200">Retorna a order solicitada</response>
    /// <response code="404">Order não encontrada</response>
    [HttpGet("({key})")]
    [EnableQuery]
    [ProducesResponseType(typeof(SingleResult<Order>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public SingleResult<Order> Get([FromODataUri] Guid key)
    {
        var result = orderRepository.GetAllQueryable().Where(o => o.Id == key);
        return SingleResult.Create(result);
    }
}
