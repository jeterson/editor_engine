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
        Assert.Equal(1, dispatcher.UndoRedoManager.UndoCount);
    }

    [Fact]
    public void UndoRedo_AddNode_Works()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();
        var node = NewLayer("Layer 1");

        dispatcher.Dispatch(new AddNodeCommand(node), context);
        Assert.True(dispatcher.Undo(context));
        Assert.Empty(document.EnumerateNodes());

        Assert.True(dispatcher.Redo(context));
        var restored = Assert.Single(document.EnumerateNodes());
        Assert.Same(node, restored);
    }

    [Fact]
    public void UndoRedo_RemoveNode_WorksAndPreservesIdentity()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();
        var first = NewLayer("A");
        var removed = NewLayer("B");
        var third = NewLayer("C");
        document.AddNode(first);
        document.AddNode(removed);
        document.AddNode(third);

        dispatcher.Dispatch(new RemoveNodeCommand(removed.Id), context);
        Assert.Equal(new[] { first.Id, third.Id }, document.EnumerateNodes().Select(x => x.Id).ToArray());

        Assert.True(dispatcher.Undo(context));
        var nodesAfterUndo = document.EnumerateNodes();
        Assert.Equal(new[] { first.Id, removed.Id, third.Id }, nodesAfterUndo.Select(x => x.Id).ToArray());
        Assert.Same(removed, nodesAfterUndo[1]);

        Assert.True(dispatcher.Redo(context));
        Assert.Equal(new[] { first.Id, third.Id }, document.EnumerateNodes().Select(x => x.Id).ToArray());
    }

    [Fact]
    public void UndoRedo_RenameNode_Works()
    {
        var document = NewDocument();
        var node = NewLayer("Old");
        document.AddNode(node);

        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        dispatcher.Dispatch(new RenameNodeCommand(node.Id, "New"), context);
        Assert.Equal("New", document.GetNode(node.Id).Name);

        Assert.True(dispatcher.Undo(context));
        Assert.Equal("Old", document.GetNode(node.Id).Name);

        Assert.True(dispatcher.Redo(context));
        Assert.Equal("New", document.GetNode(node.Id).Name);
    }

    [Fact]
    public void UndoRedo_SelectNode_Works()
    {
        var document = NewDocument();
        var first = NewLayer("A");
        var second = NewLayer("B");
        document.AddNode(first);
        document.AddNode(second);
        document.Selection.ReplaceSelection(first.Id);

        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        dispatcher.Dispatch(new SelectNodeCommand(second.Id), context);
        Assert.Equal(second.Id, document.Selection.ActiveNodeId);

        Assert.True(dispatcher.Undo(context));
        Assert.Equal(first.Id, document.Selection.ActiveNodeId);
        Assert.True(document.Selection.IsSelected(first.Id));

        Assert.True(dispatcher.Redo(context));
        Assert.Equal(second.Id, document.Selection.ActiveNodeId);
        Assert.True(document.Selection.IsSelected(second.Id));
    }

    [Fact]
    public void UndoRedo_MultipleChainedOperations_PreservesConsistency()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();
        var node = NewLayer("Layer");

        dispatcher.Dispatch(new AddNodeCommand(node), context);
        dispatcher.Dispatch(new RenameNodeCommand(node.Id, "Renamed"), context);
        dispatcher.Dispatch(new SelectNodeCommand(node.Id), context);

        Assert.True(dispatcher.Undo(context));
        Assert.True(dispatcher.Undo(context));
        Assert.True(dispatcher.Undo(context));

        Assert.Empty(document.EnumerateNodes());
        Assert.True(document.Selection.IsEmpty);

        Assert.True(dispatcher.Redo(context));
        Assert.True(dispatcher.Redo(context));
        Assert.True(dispatcher.Redo(context));

        var final = Assert.Single(document.EnumerateNodes());
        Assert.Same(node, final);
        Assert.Equal("Renamed", final.Name);
        Assert.Equal(node.Id, document.Selection.ActiveNodeId);
    }

    [Fact]
    public void Dispatch_InvalidCommand_DoesNotRecordHistory()
    {
        var document = NewDocument();
        var context = new CommandContext(document);
        var dispatcher = new CommandDispatcher();

        Assert.Throws<InvalidOperationException>(() => dispatcher.Dispatch(new RemoveNodeCommand(DocumentNodeId.New()), context));
        Assert.Empty(dispatcher.ExecutedCommands);
        Assert.Equal(0, dispatcher.UndoRedoManager.UndoCount);
    }

    private static EditorDocument NewDocument() => new(DocumentId.New(), new CanvasSize(800, 600));

    private static Layer NewLayer(string name) => new(DocumentNodeId.New(), name, true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
}
