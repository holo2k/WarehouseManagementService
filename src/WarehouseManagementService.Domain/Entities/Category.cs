namespace WarehouseManagementService.Domain.Entities;

public sealed class Category
{
    private readonly List<Product> _products = [];

    private Category()
    {
    }

    public Category(string name)
    {
        Name = name.Trim();
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
}
