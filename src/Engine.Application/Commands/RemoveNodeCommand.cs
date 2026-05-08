using Engine.Application.Commanding;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class RemoveNodeCommand : EditorCommand
{
    public RemoveNodeCommand(DocumentNodeId nodeId) : base("document.node.remove")
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        NodeId = nodeId;
    }

    public DocumentNodeId NodeId { get; }

    public override void Execute(CommandContext context)
    {
        if (!context.Document.RemoveNode(NodeId))
        {
            throw new InvalidOperationException($"Node with id '{NodeId}' was not found.");
        }
    }
}
