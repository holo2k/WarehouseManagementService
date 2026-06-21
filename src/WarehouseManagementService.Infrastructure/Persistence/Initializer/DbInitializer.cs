using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Domain.Entities;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Infrastructure.Persistence.Initializer;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (env == "Testing")
        {
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        await SeedData(dbContext, cancellationToken);
    }

    private static async Task SeedData(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var existingCategoryNames = await dbContext.Categories
            .Select(category => category.Name)
            .ToListAsync(cancellationToken);

        var categoriesToAdd = new[] { "Телевизоры", "Смартфоны", "Ноутбуки" }
            .Except(existingCategoryNames)
            .Select(name => new Category(name))
            .ToArray();

        if (categoriesToAdd.Length > 0)
        {
            dbContext.Categories.AddRange(categoriesToAdd);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = await dbContext.Categories
            .ToDictionaryAsync(category => category.Name, cancellationToken);

        dbContext.Products.AddRange(
            new Product("Samsung UE55CU7100U", "TV-SAM-55CU7100", categories["Телевизоры"].Id, ProductStatus.Active),
            new Product("LG OLED55C4RLA", "TV-LG-OLED55C4", categories["Телевизоры"].Id, ProductStatus.Defective),
            new Product("Apple iPhone 15", "PH-APL-IP15-128", categories["Смартфоны"].Id, ProductStatus.Active),
            new Product("Xiaomi Redmi Note 13", "PH-XIA-RN13-256", categories["Смартфоны"].Id, ProductStatus.WriteOff),
            new Product("Lenovo IdeaPad Slim 3", "NB-LEN-SLIM3-15", categories["Ноутбуки"].Id, ProductStatus.Defective));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
