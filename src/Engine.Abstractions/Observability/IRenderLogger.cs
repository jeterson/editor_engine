using Engine.Domain.ValueObjects;

namespace Engine.Abstractions.Observability;

public interface IRenderLogger
{
    void ExecutionStarted(int nodesCount);
    void ExecutionCompleted(int nodesCount, TimeSpan duration);
    void NodeExecutionStart(RenderNodeId id, string nodeType);
    void NodeExecutionCompleted(RenderNodeId nodeId, string nodeType, TimeSpan duration);
    void NodeCacheHit(RenderNodeId id);
    void NodeCacheMiss(RenderNodeId id);
}
