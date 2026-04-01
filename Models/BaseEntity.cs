namespace Gestao_FDC.Models;
public abstract class BaseEntity{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}