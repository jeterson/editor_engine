using Engine.Domain.ValueObjects;
using Engine.RenderGraph;

namespace Engine.Infrastructure.Contracts;

public interface IAssetResolver
{
    ValueTask<DecodedAsset> ResolveAsync(AssetReference assetReference, CancellationToken cancellationToken);
}
