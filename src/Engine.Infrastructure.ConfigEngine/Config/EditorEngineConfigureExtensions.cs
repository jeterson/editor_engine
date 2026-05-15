using Microsoft.Extensions.DependencyInjection;

namespace Engine.Infrastructure.ConfigEngine.Config;

public static class EditorEngineConfigureExtensions
{
    public static void AddEditorEngine(this IServiceCollection services, Action<IEditorEngineBuilder>? configure = null)
    {
        var builder = new EditorEngineBuilder(services);
        configure?.Invoke(builder);

        builder.UseLogger();
    }

}
