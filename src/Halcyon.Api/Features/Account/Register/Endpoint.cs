using Halcyon.Api.Data;

namespace Halcyon.Api.Features.Account.Register
{
    public static class Endpoint
    {
        public static WebApplication MapEndpoint(this WebApplication app)
        {
            app.MapPost("/manage/register", HandleAsync)
                .WithTags("Manage")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            RegisterRequest request,
            HalcyonDbContext dbContext)
        {

            return Results.Ok();
        }
    }
}
