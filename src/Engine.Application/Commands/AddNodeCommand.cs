using Engine.Application.Commanding;
using Engine.Domain.Entities;

namespace Engine.Application.Commands;

public sealed class AddNodeCommand : EditorCommand
{
    public AddNodeCommand(DocumentNode node) : base("document.node.add")
    {
        ArgumentNullException.ThrowIfNull(node);
        Node = node;
    }

    public DocumentNode Node { get; }

    public override void Execute(CommandContext context)
    {
        context.Document.AddNode(Node);
    }
}
