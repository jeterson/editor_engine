using Engine.Abstractions;

namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic decoded source asset data consumed by render backends.
/// </summary>
public sealed class DecodedAsset
{
    public DecodedAsset(int width, int height, PixelFormat pixelFormat, ReadOnlyMemory<byte> pixelBytes)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than zero.");
        }

        var expectedLength = checked(width * height * BytesPerPixel(pixelFormat));
        if (pixelBytes.Length != expectedLength)
        {
            throw new ArgumentException($"Pixel buffer length {pixelBytes.Length} does not match expected {expectedLength}.", nameof(pixelBytes));
        }

        Width = width;
        Height = height;
        PixelFormat = pixelFormat;
        PixelBytes = pixelBytes.ToArray();
    }

    public int Width { get; }

    public int Height { get; }

    public PixelFormat PixelFormat { get; }

    public ReadOnlyMemory<byte> PixelBytes { get; }

    private static int BytesPerPixel(PixelFormat pixelFormat) => pixelFormat switch
    {
        PixelFormat.Rgba8 => 4,
        PixelFormat.Bgra8 => 4,
        PixelFormat.Rgba16Float => 8,
        _ => throw new NotSupportedException($"Pixel format '{pixelFormat}' is not supported by DecodedAsset.")
    };
}
