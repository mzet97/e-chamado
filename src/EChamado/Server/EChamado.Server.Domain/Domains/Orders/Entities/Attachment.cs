using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

/// <summary>
/// Anexo associado a um comentário de chamado.
/// Armazena metadados do arquivo; conteúdo em disco ou storage.
/// </summary>
public class Attachment : SoftDeletableEntity<Attachment>
{
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public Guid CommentId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public string UploadedByUserEmail { get; private set; } = string.Empty;

    // Navigation
    public Comment Comment { get; private set; } = null!;

    private Attachment() : base(null!) { }

    private Attachment(
        Guid id,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        Guid commentId,
        Guid uploadedByUserId,
        string uploadedByUserEmail,
        IDateTimeProvider dateTimeProvider) : base(null!)
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
        CommentId = commentId;
        UploadedByUserId = uploadedByUserId;
        UploadedByUserEmail = uploadedByUserEmail;
        MarkCreated(dateTimeProvider.UtcNow);
    }

    public static Attachment Create(
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        Guid commentId,
        Guid uploadedByUserId,
        string uploadedByUserEmail,
        IDateTimeProvider dateTimeProvider)
    {
        return new Attachment(
            Guid.NewGuid(),
            fileName,
            contentType,
            sizeBytes,
            storagePath,
            commentId,
            uploadedByUserId,
            uploadedByUserEmail,
            dateTimeProvider);
    }

    public override void Validate()
    {
        // Validação básica — pode ser expandida com FluentValidation
        _isValid = !string.IsNullOrWhiteSpace(FileName) &&
                   !string.IsNullOrWhiteSpace(ContentType) &&
                   SizeBytes > 0 &&
                   SizeBytes <= 10 * 1024 * 1024; // 10MB max
        _errors = _isValid ? Enumerable.Empty<string>() : new[] { "Attachment validation failed" };
    }
}
