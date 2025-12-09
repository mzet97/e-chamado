namespace EChamado.Shared.ViewModels;

public abstract class BaseViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    protected BaseViewModel(Guid id,
                            DateTime createdAtUtc,
                            DateTime? updatedAtUtc,
                            DateTime? deletedAtUtc,
                            bool isDeleted)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        DeletedAtUtc = deletedAtUtc;
        IsDeleted = isDeleted;
    }
}
