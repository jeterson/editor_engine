using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Recurso visual original com identidade estável e metadata mínima.
/// </summary>
public sealed class ImageAsset
{
    public ImageAsset(AssetId id, string name, string mimeType)
    {
        if (id == default)
        {
            throw new ArgumentException("Asset id must be non-default.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Asset name must be provided.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new ArgumentException("Asset mime type must be provided.", nameof(mimeType));
        }

        Id = id;
        Name = name;
        MimeType = mimeType;
    }

    public AssetId Id { get; }

    public string Name { get; }

    public string MimeType { get; }
}
