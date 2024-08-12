namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Product ID is required");
    }
}

internal class DeleteProductCommandHandler
    (IDocumentSession session)
    : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        // my way of doing it
        //var product = await session.LoadAsync<Product>(command.Id, cancellationToken) ?? throw new ProductNotFoundException();
        //session.Delete(product);

        // tutorial's way
        session.Delete<Product>(command.Id);

        await session.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}
