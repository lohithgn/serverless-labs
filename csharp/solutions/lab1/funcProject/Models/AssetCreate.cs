using System.ComponentModel.DataAnnotations;

namespace FuncProject.Models;

public class AssetCreate
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0.")]
    public decimal PurchasePrice { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Asset tag must contain only uppercase letters, digits, and hyphens.")]
    public string AssetTag { get; set; } = string.Empty;

    [Required]
    public AssetType Type { get; set; } = AssetType.Other;

    [StringLength(255)]
    public string? AssignedTo { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public AssetStatus Status { get; set; } = AssetStatus.Available;
}
