
namespace Catalog.API.Exceptions;

public class ProductNotFoundException(Guid Id) : NotFoundException(nameof(Product), Id)
{
}
