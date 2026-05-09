using Engine.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;

namespace Engine.RenderGraph;

/// <summary>
/// Deterministic key used to identify a render operation result for cache reuse.
/// </summary>
public readonly record struct RenderCacheKey
{
    private RenderCacheKey(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static RenderCacheKey FromNode(RenderNode node, IReadOnlyDictionary<RenderNodeId, RenderCacheKey> dependencyKeys)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(dependencyKeys);

        var buffer = new StringBuilder();
        buffer.Append(node.GetType().Name);

        foreach (var parameter in node.GetCacheParameters())
        {
            buffer.Append('|').Append(parameter.Key).Append('=').Append(parameter.Value);
        }

        foreach (var dependencyId in node.Dependencies)
        {
            if (!dependencyKeys.TryGetValue(dependencyId, out var dependencyKey))
            {
                throw new InvalidOperationException($"Missing dependency key for node '{dependencyId}'.");
            }

            buffer.Append("|dep:").Append(dependencyId).Append('=').Append(dependencyKey.Value);
        }

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(buffer.ToString()));
        return new RenderCacheKey(Convert.ToHexString(hashBytes));
    }

    public override string ToString() => Value;
}
