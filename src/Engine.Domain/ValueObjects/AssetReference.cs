namespace Engine.Domain.ValueObjects;

/// <summary>
/// Referência lógica para um asset sem carregar conteúdo visual no domínio.
/// </summary>
public readonly record struct AssetReference
{
    public AssetReference(AssetId assetId, string path)
    {
        if (assetId == default)
        {
            throw new ArgumentException("Asset reference must target a non-default asset id.", nameof(assetId));
        }

        AssetId = assetId;
        Path = path;
    }

    public AssetId AssetId { get; }
    public string Path { get; }

    public override string ToString() => AssetId.ToString();
}
