using System.Collections.Concurrent;
using FuncProject.Models;

namespace FuncProject;

public class ProductStore
{
    private readonly ConcurrentDictionary<string, ProductResponse> _inventory = new();

    public ProductResponse? GetBySku(string sku)
    {
        _inventory.TryGetValue(sku, out var product);
        return product;
    }

    public ProductList GetAll()
    {
        return new ProductList { Items = _inventory.Values.ToList() };
    }

    public (ProductResponse? Product, bool AlreadyExists) Create(ProductCreate product)
    {
        var response = new ProductResponse
        {
            Id = Guid.NewGuid().ToString(),
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            Sku = product.Sku,
            Quantity = product.Quantity,
            Status = product.Status
        };

        if (!_inventory.TryAdd(product.Sku, response))
            return (null, true);

        return (response, false);
    }
}
