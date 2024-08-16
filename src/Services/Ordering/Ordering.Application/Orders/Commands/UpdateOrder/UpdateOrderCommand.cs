using FluentValidation;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(OrderDTO Order)
    : ICommand<UpdateOrderResult>;

public record UpdateOrderResult(bool IsSuccess);

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(c => c.Order.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(c => c.Order.OrderName).NotEmpty().WithMessage("Name is required");
        RuleFor(c => c.Order.CustomerId).NotNull().WithMessage("CustomerId is required");
    }
}
