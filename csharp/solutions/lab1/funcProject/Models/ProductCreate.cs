using System.ComponentModel.DataAnnotations;

namespace FuncProject.Models;

public class ProductCreate
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "SKU must contain only uppercase letters, digits, and hyphens.")]
    public string Sku { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 0;

    public ProductStatus Status { get; set; } = ProductStatus.Active;
}
