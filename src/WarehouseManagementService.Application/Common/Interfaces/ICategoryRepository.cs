using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken);

    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);

    void Add(Category category);
}
