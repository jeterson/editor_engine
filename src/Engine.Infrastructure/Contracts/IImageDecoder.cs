using Engine.RenderGraph;

namespace Engine.Infrastructure.Contracts;

public interface IImageDecoder
{
    bool CanDecode(string extension);
    Task<DecodedAsset> DecodeAsync(Stream stream, CancellationToken cancellationToken = default);
}
