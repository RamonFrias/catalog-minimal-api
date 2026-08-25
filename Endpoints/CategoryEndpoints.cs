using CatalogApiMinimalApi.Context;
using CatalogApiMinimalApi.Domains;
using Microsoft.EntityFrameworkCore;

namespace CatalogApiMinimalApi.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this WebApplication app)
        {
            app.MapGet("/get-all-categories", async (ApiDbContext context, CancellationToken ct = default) =>
            {
                var categories = await context.Categories.ToListAsync(ct);
                return Results.Ok(categories);
            });

            app.MapGet("/get-category-by-id/{id:Guid}", async (Guid id, ApiDbContext context, CancellationToken ct = default) => 
                        await context.Categories.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound("Category not found."));

            app.MapPost("/create-category", async (Category category, ApiDbContext context, CancellationToken ct = default) =>
            {
                var result = await context.Categories.AddAsync(category, ct);
                await context.SaveChangesAsync(ct);
                return Results.Created("/get-category-by-id/{id}", category);
            });

            app.MapPut("/update-category-by-id/{id:guid}", async (Guid id, Category category, ApiDbContext context, CancellationToken ct = default) =>
            {
                if (id != category.Id)
                    return Results.BadRequest("Informed id is different from category id.");

                var existing = await context.Categories.FindAsync(new object?[] { id }, ct);
                if (existing is null)
                    return Results.NotFound("Category not found.");

                existing.Name = category.Name;
                existing.Description = category.Description;

                await context.SaveChangesAsync(ct);
                return Results.NoContent();
            });

            app.MapDelete("/delete-category-by-id/{id:Guid}", async (Guid id, ApiDbContext context, CancellationToken ct = default) =>
            {
                if(await context.Categories.FindAsync(id) is Category category)
                {
                    context.Categories.Remove(category);
                    await context.SaveChangesAsync(ct);
                    return Results.NoContent();
                }
                return Results.NotFound("Category not found");
            });
        }
    }
}
