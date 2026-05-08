using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class RemoveNodeCommand : UndoableEditorCommand
{
    private DocumentNode? _removedNode;
    private int _removedIndex = -1;

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
        _removedIndex = context.Document.IndexOfNode(NodeId);
        _removedNode = _removedIndex >= 0 ? context.Document.GetNode(NodeId) : null;

        if (!context.Document.RemoveNode(NodeId))
        {
            throw new InvalidOperationException($"Node with id '{NodeId}' was not found.");
        }
    }

    public override void Undo(CommandContext context)
    {
        if (_removedNode is null || _removedIndex < 0)
        {
            throw new InvalidOperationException("Cannot undo remove before execute completes successfully.");
        }

        context.Document.InsertNodeAt(_removedIndex, _removedNode);
    }
}
