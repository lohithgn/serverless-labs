using System.Text.Json.Serialization;

namespace FuncProject.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
    Laptop,
    Monitor,
    Phone,
    Printer,
    Software,
    Other
}
