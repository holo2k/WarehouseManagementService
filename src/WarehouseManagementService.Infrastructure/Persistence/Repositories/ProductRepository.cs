using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Domain.Entities;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdWithCategoryAsync(
        int id,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetPageAsync(
        ProductStatus? status,
        int? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await ApplyFilters(_dbContext.Products.AsNoTracking(), status, categoryId)
            .Include(product => product.Category)
            .OrderBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        ProductStatus? status,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        return await ApplyFilters(_dbContext.Products.AsNoTracking(), status, categoryId)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AnyAsync(product => product.Sku == sku, cancellationToken);
    }

    public void Add(Product product)
    {
        _dbContext.Products.Add(product);
    }

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        ProductStatus? status,
        int? categoryId)
    {
        if (status.HasValue)
        {
            query = query.Where(product => product.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        return query;
    }
}
