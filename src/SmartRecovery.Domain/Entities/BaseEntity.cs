namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Centraliza os campos de identidade e auditoria que toda entidade deve ter.
///
/// Por que Guid e não int?
/// Guids são gerados no lado da aplicação, sem depender do banco de dados.
/// Isso facilita testes unitários (não precisa de banco para ter um Id válido)
/// e é mais seguro para APIs REST (Id não sequencial = não dá pra adivinhar).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
