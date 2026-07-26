using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Attachments;

public sealed class UploadAttachmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Upload anexo em comentário")
            .WithTags("Attachment")
            .DisableAntiforgery()
            .Produces<BaseResult<Guid>>()
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromServices] IUnitOfWork unitOfWork,
        [FromServices] IAttachmentRepository attachmentRepository,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromForm] Guid commentId,
        [FromForm] Guid userId,
        [FromForm] string userEmail,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return TypedResults.BadRequest(new BaseResult<Guid>(Guid.Empty, false, "Arquivo é obrigatório"));

        if (file.Length > 10 * 1024 * 1024)
            return TypedResults.BadRequest(new BaseResult<Guid>(Guid.Empty, false, "Arquivo excede 10MB"));

        // Verificar se o comentário existe
        var comment = await unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
            return TypedResults.BadRequest(new BaseResult<Guid>(Guid.Empty, false, "Comentário não encontrado"));

        // Salvar arquivo em disco
        var uploadsDir = Path.Combine("uploads", "attachments");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        // Criar registro no banco
        var attachment = Attachment.Create(
            file.FileName,
            file.ContentType,
            file.Length,
            filePath,
            commentId,
            userId,
            userEmail,
            dateTimeProvider);

        await attachmentRepository.AddAsync(attachment);

        return TypedResults.Ok(new BaseResult<Guid>(attachment.Id));
    }
}
