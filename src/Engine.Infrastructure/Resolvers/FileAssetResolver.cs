using Engine.Domain.ValueObjects;
using Engine.Infrastructure.Contracts;
using Engine.RenderGraph;

namespace Engine.Infrastructure.Resolvers;

internal class FileAssetResolver(IEnumerable<IImageDecoder> decoders) : IAssetResolver
{
    public async ValueTask<DecodedAsset> ResolveAsync(AssetReference assetReference, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(assetReference.Path);

        var decoder = decoders.FirstOrDefault(x => x.CanDecode(extension));

        if (decoder is null)
        {
            throw new NotSupportedException(
                $"No decoder found for extension '{extension}'.");
        }

        await using var stream = File.OpenRead(assetReference.Path);

        return await decoder.DecodeAsync(stream, cancellationToken);
    }
}
