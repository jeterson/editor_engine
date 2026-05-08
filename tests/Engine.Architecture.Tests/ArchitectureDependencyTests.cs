using System.Reflection;
using Xunit;

namespace Engine.Architecture.Tests;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void EngineDomain_DoesNotReference_Infrastructure()
    {
        AssertNoAssemblyReference(typeof(Engine.Domain.AssemblyMarker).Assembly, "Engine.Infrastructure");
    }

    [Fact]
    public void EngineDomain_DoesNotReference_Win2D()
    {
        AssertNoAssemblyReference(typeof(Engine.Domain.AssemblyMarker).Assembly, "Engine.Infrastructure.Win2D");
    }

    [Fact]
    public void EngineDomain_DoesNotReference_Skia()
    {
        AssertNoAssemblyReference(typeof(Engine.Domain.AssemblyMarker).Assembly, "Engine.Infrastructure.Skia");
    }

    [Fact]
    public void EngineDomain_DoesNotReference_WindowsApis()
    {
        var forbiddenPrefixes = new[] { "Windows", "Microsoft.Windows", "WinRT" };
        AssertNoTypeNamespacePrefix(typeof(Engine.Domain.AssemblyMarker).Assembly, forbiddenPrefixes);
    }

    [Fact]
    public void EngineRenderGraph_DoesNotReference_Presentation()
    {
        AssertNoAssemblyReference(typeof(Engine.RenderGraph.AssemblyMarker).Assembly, "Engine.Presentation");
    }

    private static void AssertNoAssemblyReference(Assembly assembly, string forbiddenAssemblyName)
    {
        var referencedAssemblyNames = assembly.GetReferencedAssemblies().Select(a => a.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain(forbiddenAssemblyName, referencedAssemblyNames);
    }

    private static void AssertNoTypeNamespacePrefix(Assembly assembly, IReadOnlyCollection<string> forbiddenPrefixes)
    {
        var offendingTypes = assembly
            .GetTypes()
            .Where(t => t.Namespace is not null && forbiddenPrefixes.Any(prefix => t.Namespace.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .Select(t => t.FullName)
            .ToArray();

        Assert.True(offendingTypes.Length == 0, $"Forbidden namespaces found: {string.Join(", ", offendingTypes)}");
    }
}
