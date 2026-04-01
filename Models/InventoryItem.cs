namespace Gestao_FDC.Models;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg"; // kg, g, l, ml, un
    public decimal MinQuantity { get; set; } = 0;
    public decimal UnitCost { get; set; }
}
