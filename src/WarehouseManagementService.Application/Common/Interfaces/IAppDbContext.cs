using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
