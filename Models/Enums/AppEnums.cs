namespace Gestao_FDC.Models.Enums;

public enum OrderType
{
    Balcao = 1,
    Mesa = 2,
    Delivery = 3
}

public enum OrderStatus
{
    Pendente = 1,
    Preparando = 2,
    Pronto = 3,
    Entregue = 4,
    Cancelado = 5
}

public enum TransactionType
{
    Receita = 1,
    Despesa = 2
}

public enum UserRole
{
    Admin = 1,
    Gerente = 2,
    Atendente = 3,
    Cozinha = 4
}

