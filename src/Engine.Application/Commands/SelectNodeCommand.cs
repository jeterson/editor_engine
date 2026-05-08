using Engine.Application.Commanding;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class SelectNodeCommand : EditorCommand
{
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
        context.Selection.ReplaceSelection(NodeId);
    }
}
