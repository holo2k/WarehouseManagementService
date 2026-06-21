using WarehouseManagementService.Domain.Entities;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Common.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdWithCategoryAsync(
        int id,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Product>> GetPageAsync(
        ProductStatus? status,
        int? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> CountAsync(
        ProductStatus? status,
        int? categoryId,
        CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken);

    void Add(Product product);
}
