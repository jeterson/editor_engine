using Engine.Abstractions.Observability;
using Engine.Runtime.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Engine.Infrastructure.Config;

public static class EngineBuilderTelemetryExtensions
{
    public static IEditorEngineBuilder UseTelemetry<T>(this IEditorEngineBuilder builder) where T : class, IRenderTelemetry
    {
        builder.Services.AddSingleton<IRenderTelemetry, T>();
        return builder;
    }
}
