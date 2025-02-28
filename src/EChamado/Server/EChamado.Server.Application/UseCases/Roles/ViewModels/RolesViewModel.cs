namespace EChamado.Server.Application.UseCases.Roles.ViewModels;

public class RolesViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public RolesViewModel()
    {

    }

    public RolesViewModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
