using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class SetNodeVisibilityCommand : UndoableEditorCommand
{
    private bool? _previousVisibility;

    public SetNodeVisibilityCommand(DocumentNodeId nodeId, bool visibility) : base("document.node.visibility")
    {
        if (nodeId == default) throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        NodeId = nodeId;
        Visibility = visibility;
    }

    public DocumentNodeId NodeId { get; }
    public bool Visibility { get; }

    public override void Execute(CommandContext context)
    {
        var node = context.Document.GetNode(NodeId);
        _previousVisibility = node.Visibility;
        node.SetVisibility(Visibility);
        context.RecordChange(new VisibilityChangedChange(NodeId));
    }

    public override void Undo(CommandContext context)
    {
        if (_previousVisibility is null) throw new InvalidOperationException("Cannot undo visibility before execute.");
        context.Document.GetNode(NodeId).SetVisibility(_previousVisibility.Value);
    }
}
