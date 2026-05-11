using Engine.Abstractions;
using Engine.Infrastructure.Contracts;
using Engine.RenderGraph;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Engine.Infrastructure.Decoders;

public class ImageDecoder : IImageDecoder
{
    public bool CanDecode(string extension)
    {
        return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<DecodedAsset> DecodeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync<Rgba32>(stream, cancellationToken);

        var pixels = new byte[image.Width * image.Height * 4];

        image.CopyPixelDataTo(pixels);

        return new DecodedAsset(
        image.Width,
        image.Height,
        PixelFormat.Rgba8,
        pixels);
    }
}
