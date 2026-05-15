using Engine.Abstractions.Observability;
using Engine.Infrastructure.Observability.Render;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Engine.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static void AddDefaultObservability(this IServiceCollection services)
    {
        services.TryAddScoped<IRenderLogger, DefaultRenderLogger>();
    }
}
