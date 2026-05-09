using Engine.Abstractions;
using Engine.Application.Commanding;
using Engine.Application.Commands;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph;
using Engine.Runtime;
using Xunit;

namespace Engine.Runtime.Tests;

public sealed class EditorSessionTests
{
    [Fact]
    public async Task Render_Lifecycle_Events_Are_Emitted()
    {
        var fixture = CreateFixture();
        var started = 0;
        var completed = 0;
        var preview = 0;

        fixture.Session.RenderStarted += (_, _) => started++;
        fixture.Session.RenderCompleted += (_, _) => completed++;
        fixture.Session.PreviewUpdated += (_, _) => preview++;

        await fixture.Session.RenderAsync();

        Assert.Equal(1, started);
        Assert.Equal(1, completed);
        Assert.Equal(1, preview);
        Assert.Equal(RenderLifecycleStatus.Completed, fixture.Session.RenderState.Status);
    }

    [Fact]
    public async Task Session_Preserves_Runtime_State_And_Cache_Across_Renders()
    {
        var fixture = CreateFixture();
        await fixture.Session.RenderAsync();
        var firstExecutions = fixture.Backend.ExecutedNodes.Count;

        await fixture.Session.RenderAsync();

        Assert.Equal(firstExecutions, fixture.Backend.ExecutedNodes.Count);
        Assert.NotNull(fixture.Session.PreviewSurface);
    }

    [Fact]
    public async Task Dispatch_And_Render_Updates_Preview_And_Invalidates_Incrementally()
    {
        var fixture = CreateFixture();
        await fixture.Session.RenderAsync();
        var originalPreviewNode = fixture.Session.PreviewSurface!.NodeId;

        var cycle = await fixture.Session.DispatchAndRenderAsync(new SetLayerTransformCommand(fixture.LayerId, new LayerTransform(2, 3, 1, 1, 0)));

        Assert.False(cycle.GraphRebuilt);
        Assert.NotEmpty(cycle.InvalidatedNodes);
        Assert.NotNull(fixture.Session.PreviewSurface);
        Assert.Equal(originalPreviewNode, fixture.Session.PreviewSurface!.NodeId);
    }

    [Fact]
    public async Task Multiple_Renders_In_Same_Session_Reuse_Coordinator_And_Executor()
    {
        var fixture = CreateFixture();

        await fixture.Session.RenderAsync();
        await fixture.Session.DispatchAndRenderAsync(new SetNodeVisibilityCommand(fixture.LayerId, false));
        await fixture.Session.DispatchAndRenderAsync(new SetNodeVisibilityCommand(fixture.LayerId, true));

        Assert.Equal(3, fixture.Backend.RenderPasses);
        Assert.Equal(RenderLifecycleStatus.Completed, fixture.Session.RenderState.Status);
    }

    private static TestFixture CreateFixture(IRenderBackend? backend = null)
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(32, 32));
        var layer = new Layer(DocumentNodeId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        document.AddNode(layer);

        var trackingBackend = backend as TrackingBackend ?? new TrackingBackend();
        var session = new EditorSession(document, new CommandDispatcher(), new RenderGraphBuilder(), new RenderGraphExecutor(trackingBackend), new InMemoryRenderCache());
        return new TestFixture(session, trackingBackend, layer.Id);
    }

    private sealed record TestFixture(EditorSession Session, TrackingBackend Backend, DocumentNodeId LayerId);

    private sealed class TrackingBackend : IRenderBackend
    {
        public int RenderPasses { get; private set; }
        public List<RenderNodeId> ExecutedNodes { get; } = new();

        public ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken = default)
        {
            RenderPasses++;
            ExecutedNodes.Add(node.Id);
            return ValueTask.FromResult(new RenderResult(node.Id, new StubSurface()));
        }
    }
}
