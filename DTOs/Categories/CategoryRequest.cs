using System.ComponentModel.DataAnnotations;

namespace Gestao_FDC.DTOs.Categories;

public class CategoryRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}
