using Engine.Abstractions.Observability;
using Engine.Infrastructure.Observability.Render;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Engine.Infrastructure.ConfigEngine.Config;

public static class EngineBuilderLoggingExtensions
{
    public static IEditorEngineBuilder UseLogger<T>(this IEditorEngineBuilder builder) where T : class, IRenderLogger
    {
        builder.Services.AddSingleton<IRenderLogger, T>();
        return builder;
    }
    public static IEditorEngineBuilder UseLogger(this IEditorEngineBuilder builder)
    {
        builder.Services.TryAddSingleton<IRenderLogger, DefaultRenderLogger>();
        return builder;
    }
}
