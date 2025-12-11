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
/// Controller OData para Comments
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]

public class CommentsController(ICommentRepository commentRepository) : ODataController
{
    /// <summary>
    /// Obtém todos os comments com suporte a queries OData
    /// </summary>
    /// <returns>Lista de comments</returns>
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 5)]
    public IQueryable<Comment> Get()
        => commentRepository.GetAllQueryable();

    /// <summary>
    /// Obtém um comment por ID
    /// </summary>
    /// <param name="key">ID do comment</param>
    /// <returns>Comment específico</returns>
    [HttpGet("({key})")]
    [EnableQuery]
    public SingleResult<Comment> Get([FromODataUri] Guid key)
    {
        var result = commentRepository.GetAllQueryable().Where(c => c.Id == key);
        return SingleResult.Create(result);
    }
}
