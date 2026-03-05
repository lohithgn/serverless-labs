using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FuncProject.Models;

namespace FuncProject;

public class ProductEndpoints
{
    private readonly ILogger<ProductEndpoints> _logger;
    private readonly ProductStore _store;

    public ProductEndpoints(ILogger<ProductEndpoints> logger, ProductStore store)
    {
        _logger = logger;
        _store = store;
    }

    [Function("AddProduct")]
    public IActionResult AddProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")]
        HttpRequest req,
        [Microsoft.Azure.Functions.Worker.Http.FromBody] ProductCreate product)
    {
        _logger.LogInformation("Processing add product request for SKU: {Sku}", product.Sku);

        var (created, alreadyExists) = _store.Create(product);

        if (alreadyExists)
        {
            _logger.LogWarning("Product already exists: {Sku}", product.Sku);
            return new ConflictObjectResult(new { detail = $"Product with SKU '{product.Sku}' already exists" });
        }

        _logger.LogInformation("Product created successfully: {Sku}", product.Sku);
        return new ObjectResult(created) { StatusCode = StatusCodes.Status201Created };
    }

    [Function("GetProduct")]
    public IActionResult GetProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{sku}")]
        HttpRequest req,
        string sku)
    {
        _logger.LogInformation("Processing get product request for SKU: {Sku}", sku);

        var product = _store.GetBySku(sku);
        if (product is null)
        {
            return new NotFoundObjectResult(new { detail = $"Product with SKU '{sku}' not found" });
        }

        return new OkObjectResult(product);
    }

    [Function("ListProducts")]
    public IActionResult ListProducts(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")]
        HttpRequest req)
    {
        _logger.LogInformation("Processing list products request");

        var products = _store.GetAll();
        return new OkObjectResult(products);
    }
}
