using Microsoft.Extensions.DependencyInjection;

namespace EESample;

internal static class ServiceLocator
{
    private static IServiceProvider? s_current;

    public static void Init(IServiceProvider serviceProvider)
    {
        s_current = serviceProvider;
    }

    public static T Get<T>() where T : class
    {
        if (s_current == null)
            throw new ArgumentException($"ServiceProvider not configured");
        return s_current.GetRequiredService<T>();
    }
}
