using FluentValidation;

namespace EChamado.Core.Domains.Orders.Validations;

public class OrderValidation : AbstractValidator<Order>
{
    public OrderValidation()
    {
        RuleFor(order => order.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(order => order.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(order => order.Evaluation)
            .MaximumLength(1000).WithMessage("Evaluation cannot exceed 1000 characters.");

        RuleFor(order => order.OpeningDate)
            .NotNull().WithMessage("Opening date is required.");

        RuleFor(order => order.StatusId)
            .NotEmpty().WithMessage("Status is required.");

        RuleFor(order => order.TypeId)
            .NotEmpty().WithMessage("Order type is required.");

        RuleFor(order => order.RequestingUserId)
            .NotEmpty().WithMessage("Requesting user ID is required.");

        RuleFor(order => order.RequestingUserEmail)
            .NotEmpty().WithMessage("Requesting user email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(200).WithMessage("Requesting user email cannot exceed 200 characters.");

        RuleFor(order => order.ResponsibleUserId)
            .NotEmpty().WithMessage("Responsible user ID is required.");

        RuleFor(order => order.ResponsibleUserEmail)
            .NotEmpty().WithMessage("Responsible user email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(200).WithMessage("Responsible user email cannot exceed 200 characters.");

        RuleFor(order => order.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(order => order.DepartmentId)
            .NotEmpty().WithMessage("Department is required.");
    }
}
