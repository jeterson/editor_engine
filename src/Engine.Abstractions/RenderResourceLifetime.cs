namespace Engine.Abstractions;

/// <summary>
/// Declares how long a render resource should be retained by backend implementations.
/// </summary>
public enum RenderResourceLifetime
{
    /// <summary>
    /// Resource may be released as soon as the current render step is completed.
    /// </summary>
    Transient = 0,

    /// <summary>
    /// Resource should be reusable within the active frame/render pass.
    /// </summary>
    Frame = 1,

    /// <summary>
    /// Resource should survive across frames for cache reuse.
    /// </summary>
    Persistent = 2
}
