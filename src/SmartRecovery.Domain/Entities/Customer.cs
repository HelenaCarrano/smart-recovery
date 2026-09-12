namespace SmartRecovery.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    /// <summary>CPF/CNPJ fictício — nunca armazenamos dados financeiros reais.</summary>
    public string Document { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Subscription> Subscriptions { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
