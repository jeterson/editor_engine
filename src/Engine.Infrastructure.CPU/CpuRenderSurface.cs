using Engine.Abstractions;

namespace Engine.Infrastructure.CPU;

public sealed class CpuRenderSurface : IRenderSurface
{
    private readonly byte[] _pixelBytes;

    public CpuRenderSurface(RenderSurfaceDescriptor descriptor, byte[] pixelBytes)
    {
        Descriptor = descriptor;
        _pixelBytes = pixelBytes ?? throw new ArgumentNullException(nameof(pixelBytes));

        var expectedLength = descriptor.Width * descriptor.Height * BytesPerPixel(descriptor.PixelFormat);
        if (_pixelBytes.Length != expectedLength)
        {
            throw new ArgumentException($"Pixel buffer length {_pixelBytes.Length} does not match expected {expectedLength}.", nameof(pixelBytes));
        }
    }

    public RenderSurfaceDescriptor Descriptor { get; }

    public ReadOnlyMemory<byte> PixelBytes => _pixelBytes;

    public Span<byte> GetWritablePixelSpan() => _pixelBytes;

    public CpuRenderSurface Clone() => new(Descriptor, [.. _pixelBytes]);

    public static CpuRenderSurface CreateEmpty(int width, int height)
    {
        var descriptor = new RenderSurfaceDescriptor(width, height, PixelFormat.Rgba8, isHighPrecision: false, RenderResourceLifetime.Transient);
        return new CpuRenderSurface(descriptor, new byte[width * height * 4]);
    }

    private static int BytesPerPixel(PixelFormat pixelFormat) => pixelFormat switch
    {
        PixelFormat.Rgba8 => 4,
        PixelFormat.Bgra8 => 4,
        PixelFormat.Rgba16Float => 8,
        _ => throw new NotSupportedException($"Pixel format '{pixelFormat}' is not supported by CpuRenderSurface.")
    };
}
