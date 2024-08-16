using Ordering.Application.Orders.Queries.GetOrdersByName;

namespace Ordering.API.Endpoints;

//public record GetOrderByNameRequest(string Name);

public record GetOrderByNameResponse(IEnumerable<OrderDTO> Orders);

public class GetOrdersByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{orderName}", async (string orderName, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByNameQuery(orderName));
            var response = result.Adapt<GetOrderByNameResponse>();
            return Results.Ok(response);
        })
        .WithName("GetOrdersByName")
        .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Orders By Name")
        .WithDescription("Get Orders By Name");
    }
}
