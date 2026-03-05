using System.Text.Json.Serialization;

namespace FuncProject.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductStatus
{
    Active,
    Inactive
}
