using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;

namespace Engine.Runtime;

public sealed class EditorSession
{
    private readonly EditorRenderCoordinator _coordinator;

    public EditorSession(
        EditorDocument document,
        CommandDispatcher dispatcher,
        RenderGraphBuilder graphBuilder,
        RenderGraphExecutor executor,
        IRenderCache cache)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        CommandContext = new CommandContext(Document);
        _coordinator = new EditorRenderCoordinator(dispatcher, graphBuilder, executor, Cache, Document);
        RenderState = CurrentRenderState.Idle;
    }

    public EditorDocument Document { get; }
    public CommandContext CommandContext { get; }
    public IRenderCache Cache { get; }
    public CurrentRenderState RenderState { get; private set; }
    public CurrentPreviewSurface? PreviewSurface { get; private set; }
    public Engine.RenderGraph.RenderGraph CurrentGraph => _coordinator.CurrentGraph;

    public event EventHandler<RenderStarted>? RenderStarted;
    public event EventHandler<RenderCompleted>? RenderCompleted;
    public event EventHandler<RenderFailed>? RenderFailed;
    public event EventHandler<PreviewUpdated>? PreviewUpdated;

    public ValueTask<EditorRenderCycleResult> RenderAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteRenderAsync(() => _coordinator.ProcessChangesAndRenderAsync(Array.Empty<DocumentChange>(), Document, cancellationToken), cancellationToken);
    }

    public ValueTask<EditorRenderCycleResult> DispatchAndRenderAsync(EditorCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return ExecuteRenderAsync(() => _coordinator.DispatchAndRenderAsync(command, CommandContext, cancellationToken), cancellationToken);
    }

    private async ValueTask<EditorRenderCycleResult> ExecuteRenderAsync(
        Func<ValueTask<EditorRenderCycleResult>> renderAction,
        CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        RenderState = RenderState with
        {
            Status = RenderLifecycleStatus.Rendering,
            StartedAtUtc = startedAt,
            CompletedAtUtc = null,
            FailureReason = null
        };
        RenderStarted?.Invoke(this, new RenderStarted(startedAt, Array.Empty<Engine.Domain.ValueObjects.RenderNodeId>(), false));

        try
        {
            var cycleResult = await renderAction();
            cancellationToken.ThrowIfCancellationRequested();

            var completedAt = DateTimeOffset.UtcNow;
            var executedNodes = cycleResult.Results.Keys.ToArray();
            RenderState = new CurrentRenderState(
                RenderLifecycleStatus.Completed,
                cycleResult.GraphRebuilt,
                cycleResult.InvalidatedNodes,
                executedNodes,
                startedAt,
                completedAt,
                null);

            RenderCompleted?.Invoke(this, new RenderCompleted(startedAt, completedAt, cycleResult.Results, cycleResult.InvalidatedNodes, cycleResult.GraphRebuilt));

            var preview = ResolvePreview(cycleResult.Results, completedAt);
            if (preview is not null)
            {
                PreviewSurface = preview;
                PreviewUpdated?.Invoke(this, new PreviewUpdated(preview));
            }

            return cycleResult;
        }
        catch (Exception ex)
        {
            var failedAt = DateTimeOffset.UtcNow;
            RenderState = RenderState with
            {
                Status = RenderLifecycleStatus.Failed,
                CompletedAtUtc = failedAt,
                FailureReason = ex.Message
            };

            RenderFailed?.Invoke(this, new RenderFailed(startedAt, failedAt, ex, Array.Empty<Engine.Domain.ValueObjects.RenderNodeId>(), false));
            throw;
        }
    }

    private CurrentPreviewSurface? ResolvePreview(IReadOnlyDictionary<Engine.Domain.ValueObjects.RenderNodeId, RenderResult> results, DateTimeOffset updatedAt)
    {
        if (results.Count == 0)
        {
            return PreviewSurface;
        }

        var finalNodeId = CurrentGraph.ExecutionOrder.LastOrDefault();
        if (finalNodeId != default && results.TryGetValue(finalNodeId, out var finalResult))
        {
            return new CurrentPreviewSurface(finalNodeId, finalResult.Surface, updatedAt);
        }

        var fallback = results.Last();
        return new CurrentPreviewSurface(fallback.Key, fallback.Value.Surface, updatedAt);
    }
}
