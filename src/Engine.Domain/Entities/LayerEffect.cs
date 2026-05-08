using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Representa uma operação visual declarativa aplicada a uma layer.
/// Não executa renderização e é independente de backend gráfico.
/// </summary>
public sealed class LayerEffect
{
    private readonly Dictionary<string, object?> _parameters;

    public LayerEffect(EffectId id, string kind, bool isEnabled, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        if (id == default)
        {
            throw new ArgumentException("Effect id must be non-default.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(kind))
        {
            throw new ArgumentException("Effect kind must be provided.", nameof(kind));
        }

        Id = id;
        Kind = kind;
        IsEnabled = isEnabled;
        _parameters = parameters is null
            ? new Dictionary<string, object?>(StringComparer.Ordinal)
            : new Dictionary<string, object?>(parameters, StringComparer.Ordinal);
    }

    public EffectId Id { get; }

    public string Kind { get; }

    public bool IsEnabled { get; private set; }

    public IReadOnlyDictionary<string, object?> Parameters => _parameters;

    public void SetEnabled(bool isEnabled) => IsEnabled = isEnabled;
}
