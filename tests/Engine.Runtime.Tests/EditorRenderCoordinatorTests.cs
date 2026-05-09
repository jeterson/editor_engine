using Engine.Application.Commanding;
using Engine.Application.Commands;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph;
using Engine.Abstractions;
using Engine.Runtime;
using Xunit;
using Engine.RenderGraph.Abstractions;

namespace Engine.Runtime.Tests;

public sealed class EditorRenderCoordinatorTests
{
    [Fact]
    public async Task Command_To_DocumentChange_Triggers_IncrementalInvalidation()
    {
        var fixture = CreateFixture();
        await fixture.Coordinator.ProcessChangesAndRenderAsync(Array.Empty<DocumentChange>(), fixture.Context.Document);

        var result = await fixture.Coordinator.DispatchAndRenderAsync(
            new SetLayerTransformCommand(fixture.LayerId, new LayerTransform(10, 0, 1, 1, 0)), fixture.Context);

        Assert.False(result.GraphRebuilt);
        Assert.NotEmpty(result.InvalidatedNodes);
        Assert.Contains(fixture.Coordinator.CurrentGraph.RenderNodesByDocumentNode[fixture.LayerId], id => result.InvalidatedNodes.Contains(id));
    }

    [Fact]
    public async Task NodeRemoved_Triggers_GraphRebuild_And_Rerender()
    {
        var fixture = CreateFixture();
        var baseline = fixture.Coordinator.CurrentGraph.ExecutionOrder.Count;

        var result = await fixture.Coordinator.DispatchAndRenderAsync(new RemoveNodeCommand(fixture.LayerId), fixture.Context);

        Assert.True(result.GraphRebuilt);
        Assert.True(fixture.Coordinator.CurrentGraph.ExecutionOrder.Count < baseline);
    }

    [Fact]
    public async Task EffectParameterChanged_Uses_IncrementalInvalidation()
    {
        var fixture = CreateFixture();
        var effectId = EffectId.New();

        var result = await fixture.Coordinator.ProcessChangesAndRenderAsync(
            new DocumentChange[] { new EffectParameterChangedChange(fixture.LayerId, effectId) }, fixture.Context.Document);

        Assert.False(result.GraphRebuilt);
        Assert.NotEmpty(result.InvalidatedNodes);
    }

    [Fact]
    public async Task PartialRerender_ReusesCache_ForNonInvalidatedNodes()
    {
        var fixture = CreateFixture();
        await fixture.Coordinator.ProcessChangesAndRenderAsync(Array.Empty<DocumentChange>(), fixture.Context.Document);
        var firstExecutions = fixture.Backend.ExecutedNodes.Count;

        await fixture.Coordinator.DispatchAndRenderAsync(
            new SetNodeVisibilityCommand(fixture.LayerId, false), fixture.Context);

        Assert.True(fixture.Backend.ExecutedNodes.Count < firstExecutions * 2);
    }

    private static TestFixture CreateFixture()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        var layer = new Layer(DocumentNodeId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        document.AddNode(layer);
        var context = new CommandContext(document);
        var backend = new TrackingBackend();
        var coordinator = new EditorRenderCoordinator(new CommandDispatcher(), new RenderGraphBuilder(), new RenderGraphExecutor(backend), new InMemoryRenderCache(), document);
        return new TestFixture(context, coordinator, backend, layer.Id);
    }

    private sealed record TestFixture(CommandContext Context, EditorRenderCoordinator Coordinator, TrackingBackend Backend, DocumentNodeId LayerId);

    private sealed class TrackingBackend : IRenderBackend
    {
        public List<RenderNodeId> ExecutedNodes { get; } = new();
        public ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken = default)
        {
            ExecutedNodes.Add(node.Id);
            return ValueTask.FromResult(new RenderResult(node.Id, new StubSurface()));
        }
    }
}


internal sealed class StubSurface : IRenderSurface
{
    public RenderSurfaceDescriptor Descriptor => new(1, 1, PixelFormat.Rgba8);
}
