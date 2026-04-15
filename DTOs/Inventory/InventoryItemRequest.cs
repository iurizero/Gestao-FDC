using System.ComponentModel.DataAnnotations;

namespace Gestao_FDC.DTOs.Inventory;

public class InventoryItemRequest
{
    [Required(ErrorMessage = "O nome do item é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "A quantidade deve ser um valor positivo.")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
    public string Unit { get; set; } = "un";

    [Range(0, 1000000, ErrorMessage = "A quantidade mínima deve ser um valor positivo.")]
    public decimal MinQuantity { get; set; }

    [Range(0, 1000000, ErrorMessage = "O custo unitário deve ser um valor positivo.")]
    public decimal UnitCost { get; set; }
}
