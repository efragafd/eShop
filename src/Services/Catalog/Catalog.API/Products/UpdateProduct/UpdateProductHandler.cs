﻿namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithMessage("Product ID is required");

        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");


        RuleFor(r => r.Category).NotEmpty().WithMessage("Category is required");

        RuleFor(r => r.ImageFile).NotEmpty().WithMessage("ImageFile is required");

        RuleFor(r => r.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}

internal class UpdateProductCommandHandler
    (IDocumentSession session, ILogger<UpdateProductCommandHandler> logger)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateProductCommandHandler.Handle called with {@Cmd}", command);

        var product = await session.LoadAsync<Product>(command.Id, cancellationToken) ?? throw new ProductNotFoundException(command.Id);

        product.Name = command.Name;
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
