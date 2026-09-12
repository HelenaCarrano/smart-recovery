namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Id é Guid (não int) porque é gerado no lado da aplicação, sem depender do banco —
/// facilita testes unitários e evita Ids sequenciais adivinháveis em APIs REST.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
