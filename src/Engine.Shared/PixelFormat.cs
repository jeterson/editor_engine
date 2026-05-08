namespace Engine.Shared;

/// <summary>
/// Defines pixel formats supported by the engine.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 8-bit per channel RGBA.
    /// </summary>
    Rgba8 = 0,

    /// <summary>
    /// 8-bit per channel BGRA.
    /// </summary>
    Bgra8 = 1,

    /// <summary>
    /// 16-bit floating-point per channel RGBA.
    /// </summary>
    Rgba16Float = 2
}
