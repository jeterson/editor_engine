using Engine.Abstractions.Observability;
using Engine.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Engine.Infrastructure.Observability.Render;

public class DefaultRenderLogger(ILogger<DefaultRenderLogger> logger) : IRenderLogger
{
    public void ExecutionCompleted(int nodesCount, TimeSpan duration)
    {
        logger.LogInformation("{NodeCount}(s) executed in {Duration}", nodesCount, duration);
    }

    public void ExecutionStarted(int nodesCount)
    {
        logger.LogInformation("Starting execution with {NodeCount}(s)", nodesCount);
    }

    public void NodeCacheHit(RenderNodeId id)
    {
        logger.LogInformation("Node {NodeId} cache hitted", id);
    }

    public void NodeCacheMiss(RenderNodeId id)
    {
        logger.LogInformation("Node {NodeId} cache miss, Node will be executed", id);
    }

    public void NodeExecutionCompleted(RenderNodeId nodeId, string nodeType, TimeSpan duration)
    {
        logger.LogInformation("Node {NodeType}:{NodeId} execution completed. Duration: {Duration}", nodeType, nodeId, duration);
    }

    public void NodeExecutionStart(RenderNodeId id, string nodeType)
    {
        logger.LogInformation("Node {NodeType}:{NodeId} execution started...", nodeType, id);
    }
}
