namespace Gestao_FDC.Models;

public class Salgado : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}