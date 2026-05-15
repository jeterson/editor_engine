using Engine.Abstractions.Observability;
using Microsoft.Extensions.DependencyInjection;

namespace Engine.Infrastructure.ConfigEngine.Config;

public static class EngineBuilderTelemetryExtensions
{
    public static IEditorEngineBuilder UseTelemetry<T>(this IEditorEngineBuilder builder) where T : class, IRenderTelemetry
    {
        builder.Services.AddSingleton<IRenderTelemetry, T>();
        return builder;
    }
}
