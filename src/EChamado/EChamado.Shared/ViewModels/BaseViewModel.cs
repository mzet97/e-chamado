namespace EChamado.Shared.ViewModels;

public abstract class BaseViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected BaseViewModel(Guid id,
                            DateTime createdAt,
                            DateTime? updatedAt,
                            DateTime? deletedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
    }
}
