using Engine.Application.Commanding;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class SelectNodeCommand : UndoableEditorCommand
{
    private List<DocumentNodeId>? _previousSelection;
    private DocumentNodeId? _previousActiveNodeId;

    public SelectNodeCommand(DocumentNodeId nodeId) : base("document.node.select")
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
        _ = context.Document.GetNode(NodeId);
        _previousSelection = context.Selection.SelectedNodeIds.ToList();
        _previousActiveNodeId = context.Selection.ActiveNodeId;
        context.Selection.ReplaceSelection(NodeId);
    }

    public override void Undo(CommandContext context)
    {
        if (_previousSelection is null)
        {
            throw new InvalidOperationException("Cannot undo selection before execute completes successfully.");
        }

        context.Selection.RestoreSelection(_previousSelection, _previousActiveNodeId);
    }
}
