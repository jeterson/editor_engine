using Engine.Application.Commanding;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class RenameNodeCommand : UndoableEditorCommand
{
    private string? _previousName;

    public RenameNodeCommand(DocumentNodeId nodeId, string newName) : base("document.node.rename")
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Node name must be provided.", nameof(newName));
        }

        NodeId = nodeId;
        NewName = newName;
    }

    public DocumentNodeId NodeId { get; }

    public string NewName { get; }

    public override void Execute(CommandContext context)
    {
        var node = context.Document.GetNode(NodeId);
        _previousName = node.Name;
        node.Rename(NewName);
    }

    public override void Undo(CommandContext context)
    {
        if (_previousName is null)
        {
            throw new InvalidOperationException("Cannot undo rename before execute completes successfully.");
        }

        var node = context.Document.GetNode(NodeId);
        node.Rename(_previousName);
    }
}
