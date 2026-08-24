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
                Results.Ok(categories);
            });

            app.MapGet("/get-categorie-by-id/{id:int}", async (int id, ApiDbContext context, CancellationToken ct = default) => 
                        await context.Categories.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound("Category not found."));

            app.MapPost("/create-category", async (Category category, ApiDbContext context, CancellationToken ct = default) =>
            {
                var result = await context.Categories.AddAsync(category, ct);
                Results.CreatedAtRoute("/get-category-by-id/{id}", category);
            });

            app.MapPut("/update-category-by-id/{id:Guid}", async (Guid id, Category category, ApiDbContext context, CancellationToken ct = default) =>
            {
                if(id == category.Id)
                {
                    if (await context.Categories.FindAsync(id, ct) is not null)
                    {
                        context.Categories.Update(category);
                        await context.SaveChangesAsync(ct);
                        return Results.NoContent();
                    }
                    return Results.NotFound("Category not found.");
                }
                return Results.BadRequest("Informed id is diferent of category id.");
            });

            app.MapDelete("/delete-category-by-id", async (Guid id, ApiDbContext context, CancellationToken ct = default) =>
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
