namespace Engine.Abstractions;

/// <summary>
/// Represents a rendered surface regardless of backend implementation details.
/// </summary>
public interface IRenderSurface : IDisposable
{
    RenderSurfaceDescriptor Descriptor { get; }
}
