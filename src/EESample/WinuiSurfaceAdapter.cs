using Engine.Infrastructure.CPU;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

namespace EESample;

public static class WinuiSurfaceAdapter
{
    public static SoftwareBitmap ToSoftwareBitmap(this CpuRenderSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        var descriptor = surface.Descriptor;

        byte[] pixels = surface.PixelBytes.ToArray();

        // RGBA -> BGRA
        for (int i = 0; i < pixels.Length; i += 4)
        {
            (pixels[i], pixels[i + 2]) =
                (pixels[i + 2], pixels[i]);
        }

        var bitmap = new SoftwareBitmap(
            BitmapPixelFormat.Bgra8,
            descriptor.Width,
            descriptor.Height,
            BitmapAlphaMode.Premultiplied);

        bitmap.CopyFromBuffer(pixels.AsBuffer());

        return bitmap;

    }
}
