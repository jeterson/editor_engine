using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Representa uma camada do documento.
/// </summary>
public sealed class Layer : DocumentNode
{
    public Layer(
        DocumentNodeId id,
        string name,
        bool visibility,
        LayerTransform transform,
        Opacity opacity,
        BlendMode blendMode,
        AssetReference assetReference) : base(id, name, visibility)
    {
        if (id == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(id));
        }

        ValidateBlendMode(blendMode);

        if (assetReference.AssetId == default)
        {
            throw new ArgumentException("Layer asset reference must target a non-default asset id.", nameof(assetReference));
        }

        Transform = transform;
        Opacity = opacity;
        BlendMode = blendMode;
        AssetReference = assetReference;
        EffectStack = new EffectStack();
    }

    public LayerTransform Transform { get; private set; }

    public Opacity Opacity { get; private set; }

    public BlendMode BlendMode { get; private set; }

    public AssetReference AssetReference { get; private set; }

    public EffectStack EffectStack { get; }

    public void SetTransform(LayerTransform transform) => Transform = transform;

    public void SetOpacity(Opacity opacity) => Opacity = opacity;

    public void SetAssetReference(AssetReference assetReference)
    {
        if (assetReference.AssetId == default)
        {
            throw new ArgumentException("Layer asset reference must target a non-default asset id.", nameof(assetReference));
        }

        AssetReference = assetReference;
    }

    public void SetBlendMode(BlendMode blendMode)
    {
        ValidateBlendMode(blendMode);
        BlendMode = blendMode;
    }
    private static void ValidateBlendMode(BlendMode blendMode)
    {
        if (!Enum.IsDefined(blendMode))
        {
            throw new ArgumentOutOfRangeException(nameof(blendMode), "Unsupported blend mode.");
        }
    }
}
