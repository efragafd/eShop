using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDTO BasketCheckoutDTO) 
    : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(c => c.BasketCheckoutDTO).NotNull().WithMessage("BasketCheckoutDTO cannot be null");
        RuleFor(c => c.BasketCheckoutDTO.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler
    (IBasketRepository repository, IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // get existing basket with total price
        var basket = await repository.GetBasketAsync(command.BasketCheckoutDTO.UserName, cancellationToken);
        if (basket is null) return new CheckoutBasketResult(false);

        // set total price on basket checkout event message
        var eventMessage = command.BasketCheckoutDTO.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;

        // send basket checkout event to rabbitmq using mass transit
        await publishEndpoint.Publish(eventMessage, cancellationToken);

        // delete de basket
        await repository.DeleteBasketAsync(command.BasketCheckoutDTO.UserName, cancellationToken);
        
        return new CheckoutBasketResult(true);
    }
}
