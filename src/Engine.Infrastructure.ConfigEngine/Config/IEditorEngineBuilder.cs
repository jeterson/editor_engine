using Microsoft.Extensions.DependencyInjection;

namespace Engine.Infrastructure.ConfigEngine.Config;

public interface IEditorEngineBuilder
{
    IServiceCollection Services { get; }
}
internal class EditorEngineBuilder : IEditorEngineBuilder
{
    public IServiceCollection Services { get; }

    public EditorEngineBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
