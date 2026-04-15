using System.ComponentModel.DataAnnotations;

namespace Gestao_FDC.DTOs.Products;

public class ProductRequest
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0.01, 10000, ErrorMessage = "O preço deve ser entre 0.01 e 10.000.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public int CategoryId { get; set; }

    [Range(0, 1000000, ErrorMessage = "A quantidade em estoque deve ser zero ou positiva.")]
    public int StockQuantity { get; set; }

    public bool TrackStock { get; set; } = true;

    public string? ImageUrl { get; set; }
}
