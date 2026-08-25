using CatalogApiMinimalApi.Context;
using CatalogApiMinimalApi.Domains;
using Microsoft.EntityFrameworkCore;

namespace CatalogApiMinimalApi.Endpoints
{
    public static class ProductEndpoint
    {
        public static void MapProductsEndpoints(this WebApplication app)
        {
            app.MapGet("/get-all-products", async (ApiDbContext context, CancellationToken ct = default) =>
            {
                var products = await context.Products.ToListAsync();
                return Results.Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Data = products is null ? new List<Product>() : products
                });
            });

            app.MapGet("/get-product-by-id/{id:Guid}", async (Guid id, ApiDbContext context, CancellationToken ct = default) => 
                                    await context.Products.FindAsync(id) is Product product ? Results.Ok(product) : 
                                    Results.NotFound(NotFoundProducrMethod()));

            app.MapPost("/create-product", async (Product product, ApiDbContext context, CancellationToken ct = default) =>
            {
                await context.Products.AddAsync(product, ct);
                await context.SaveChangesAsync(ct);
                return Results.Created("/get-product-by-id/{id}", product);
            });

            app.MapPut("/update-product/{id:Guid}", async (Guid id, Product product, ApiDbContext context, CancellationToken ct) =>
            {
                if(id == product.Id)
                {
                    if(await context.Products.FindAsync(id, ct) is not null)
                    {
                        context.Products.Update(product);
                        await context.SaveChangesAsync(ct);
                        return Results.NoContent();
                    }
                    return Results.NotFound(NotFoundProducrMethod());
                }
                return Results.BadRequest("Id is diferent of product id.");
            });

            app.MapDelete("/delete-product-by-id/{id:guid}", async (Guid id, ApiDbContext context, CancellationToken ct = default) =>
            {
                if(await context.Products.FindAsync(id, ct) is Product product)
                {
                    context.Products.Remove(product);
                    await context.SaveChangesAsync(ct);
                    return Results.NoContent();
                }
                return Results.NotFound(NotFoundProducrMethod());
            });
        }
        private static object NotFoundProducrMethod()
        {
            return new
            {
                Status = StatusCodes.Status404NotFound,
                Data = "Product not found."
            };
        }
    }
}
