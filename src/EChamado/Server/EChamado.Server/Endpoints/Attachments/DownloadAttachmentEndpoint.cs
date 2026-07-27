using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Repositories.Orders;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Attachments;

public sealed class DownloadAttachmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Download anexo")
            .WithTags("Attachment")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        [FromServices] IAttachmentRepository attachmentRepository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var attachment = await attachmentRepository.GetByIdAsync(id);
        if (attachment == null || attachment.IsDeleted)
            return TypedResults.NotFound();

        if (!System.IO.File.Exists(attachment.StoragePath))
            return TypedResults.NotFound();

        var stream = new FileStream(attachment.StoragePath, FileMode.Open, FileAccess.Read);
        return TypedResults.File(stream, attachment.ContentType, attachment.FileName);
    }
}
