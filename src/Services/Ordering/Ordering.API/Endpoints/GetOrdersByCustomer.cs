using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints;

//public record GetOrderByCustomerRequest(string CustomerId);

public record GetOrderByCustomerResponse(IEnumerable<OrderDTO> Orders);

public class GetOrdersByCustomer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/customer/{customerId}", async (Guid customerId, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));
            var response = result.Adapt<GetOrderByCustomerResponse>();
            return Results.Ok(response);
        })
        .WithName("GetOrdersByCustomer")
        .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Orders By Customer")
        .WithDescription("Get Orders By Customer");
    }
}
