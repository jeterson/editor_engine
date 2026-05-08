using Engine.Application.Commanding;
using Engine.Application.Commands;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Application.Tests.Commanding;

public sealed class CommandDispatcherTests
{
    [Fact]
    public void Dispatch_ExecutesAddAndTracksHistory()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();
        var node = NewLayer("Layer 1");

        dispatcher.Dispatch(new AddNodeCommand(node), context);

        Assert.Single(document.EnumerateNodes());
        Assert.Single(dispatcher.ExecutedCommands);
        Assert.Equal("document.node.add", dispatcher.ExecutedCommands[0].SemanticId);
    }

    [Fact]
    public void Dispatch_RenameNode_ChangesDocumentState()
    {
        var document = NewDocument();
        var node = NewLayer("Old");
        document.AddNode(node);

        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        dispatcher.Dispatch(new RenameNodeCommand(node.Id, "New"), context);

        Assert.Equal("New", document.GetNode(node.Id).Name);
    }

    [Fact]
    public void Dispatch_SelectNode_ChangesSelection()
    {
        var document = NewDocument();
        var first = NewLayer("A");
        var second = NewLayer("B");
        document.AddNode(first);
        document.AddNode(second);

        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        dispatcher.Dispatch(new SelectNodeCommand(second.Id), context);

        Assert.Equal(second.Id, document.Selection.ActiveNodeId);
        Assert.True(document.Selection.IsSelected(second.Id));
        Assert.False(document.Selection.IsSelected(first.Id));
    }

    [Fact]
    public void Dispatch_RemoveNode_RemovesNodeAndSelection()
    {
        var document = NewDocument();
        var node = NewLayer("Remove me");
        document.AddNode(node);
        document.Selection.ReplaceSelection(node.Id);

        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        dispatcher.Dispatch(new RemoveNodeCommand(node.Id), context);

        Assert.Empty(document.EnumerateNodes());
        Assert.True(document.Selection.IsEmpty);
    }

    [Fact]
    public void Dispatch_InvalidCommand_DoesNotRecordHistory()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        Assert.Throws<InvalidOperationException>(() => dispatcher.Dispatch(new RemoveNodeCommand(DocumentNodeId.New()), context));
        Assert.Empty(dispatcher.ExecutedCommands);
    }

    private static EditorDocument NewDocument() => new(DocumentId.New(), new CanvasSize(800, 600));

    private static Layer NewLayer(string name) => new(DocumentNodeId.New(), name, true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
}
