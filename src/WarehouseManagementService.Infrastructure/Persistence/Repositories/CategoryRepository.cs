using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AnyAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AnyAsync(category => category.Name == name, cancellationToken);
    }

    public void Add(Category category)
    {
        _dbContext.Categories.Add(category);
    }
}
