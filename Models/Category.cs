using System.Text.Json.Serialization;

namespace Gestao_FDC.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
