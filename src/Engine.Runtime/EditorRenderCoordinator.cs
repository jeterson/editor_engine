using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;
using RenderGraphClass = Engine.RenderGraph.RenderGraph;

namespace Engine.Runtime;

public sealed class EditorRenderCoordinator
{
    private readonly CommandDispatcher _dispatcher;
    private readonly RenderGraphBuilder _graphBuilder;
    private readonly RenderGraphExecutor _executor;
    private readonly IRenderCache _cache;

    private RenderGraphClass _graph;
    private RenderInvalidationManager _invalidationManager;

    public EditorRenderCoordinator(
        CommandDispatcher dispatcher,
        RenderGraphBuilder graphBuilder,
        RenderGraphExecutor executor,
        IRenderCache cache,
        EditorDocument document)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _graphBuilder = graphBuilder ?? throw new ArgumentNullException(nameof(graphBuilder));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        ArgumentNullException.ThrowIfNull(document);

        _graph = _graphBuilder.Build(document);
        _invalidationManager = new RenderInvalidationManager(_graph);
    }

    public RenderGraphClass CurrentGraph => _graph;

    public async ValueTask<EditorRenderCycleResult> DispatchAndRenderAsync(EditorCommand command, CommandContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(context);

        context.ClearChanges();
        _dispatcher.Dispatch(command, context);
        return await ProcessChangesAndRenderAsync(context.Changes, context.Document, cancellationToken);
    }

    public async ValueTask<EditorRenderCycleResult> ProcessChangesAndRenderAsync(
        IEnumerable<DocumentChange> changes,
        EditorDocument document,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(document);

        var materializedChanges = changes.ToArray();
        var requiresGraphRebuild = materializedChanges.Any(RequiresGraphRebuild);

        IReadOnlyCollection<RenderNodeId> invalidatedNodes;
        if (requiresGraphRebuild)
        {
            _graph = _graphBuilder.Build(document);
            _invalidationManager = new RenderInvalidationManager(_graph);
            invalidatedNodes = _graph.ExecutionOrder.ToArray();
        }
        else
        {
            invalidatedNodes = _invalidationManager.InvalidateFromDocumentChanges(materializedChanges);
        }

        var context = new RenderExecutionContext(_cache, invalidatedNodes);
        var results = await _executor.ExecuteAsync(_graph, context, cancellationToken);
        return new EditorRenderCycleResult(requiresGraphRebuild, invalidatedNodes, results);
    }

    private static bool RequiresGraphRebuild(DocumentChange change)
        => change is NodeAddedChange or NodeRemovedChange or EffectChangedChange;
}

public sealed record EditorRenderCycleResult(
    bool GraphRebuilt,
    IReadOnlyCollection<RenderNodeId> InvalidatedNodes,
    IReadOnlyDictionary<RenderNodeId, RenderResult> Results);
