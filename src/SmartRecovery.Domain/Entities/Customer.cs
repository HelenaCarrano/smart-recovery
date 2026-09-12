namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Representa um cliente com assinaturas na plataforma.
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Documento fictício (CPF ou CNPJ simulado).
    /// Nunca armazenamos dados financeiros reais.
    /// </summary>
    public string Document { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // ── Navegação ─────────────────────────────────────────────────────────────
    // O EF Core usa essas propriedades para montar os JOINs automaticamente.
    // ICollection permite adicionar/remover itens sem substituir a coleção inteira.

    public ICollection<Subscription> Subscriptions { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
