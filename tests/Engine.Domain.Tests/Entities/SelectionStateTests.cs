using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class SelectionStateTests
{
    [Fact]
    public void ReplaceSelection_SelectsSingleNodeAndSetsActive()
    {
        var selection = new SelectionState();
        var first = DocumentNodeId.New();
        var second = DocumentNodeId.New();

        selection.AddToSelection(first);
        selection.ReplaceSelection(second);

        Assert.Single(selection.SelectedNodeIds);
        Assert.Equal(second, selection.SelectedNodeIds[0]);
        Assert.Equal(second, selection.ActiveNodeId);
    }

    [Fact]
    public void AddToSelection_SupportsMultiSelectionWithoutDuplication()
    {
        var selection = new SelectionState();
        var first = DocumentNodeId.New();
        var second = DocumentNodeId.New();

        var addedFirst = selection.AddToSelection(first);
        var addedSecond = selection.AddToSelection(second);
        var addedDuplicate = selection.AddToSelection(first);

        Assert.True(addedFirst);
        Assert.True(addedSecond);
        Assert.False(addedDuplicate);
        Assert.Equal(2, selection.SelectedNodeIds.Count);
        Assert.Contains(first, selection.SelectedNodeIds);
        Assert.Contains(second, selection.SelectedNodeIds);
    }

    [Fact]
    public void ToggleSelection_AddsAndRemovesNode()
    {
        var selection = new SelectionState();
        var nodeId = DocumentNodeId.New();

        var firstToggle = selection.ToggleSelection(nodeId);
        var secondToggle = selection.ToggleSelection(nodeId);

        Assert.True(firstToggle);
        Assert.True(secondToggle);
        Assert.Empty(selection.SelectedNodeIds);
        Assert.Null(selection.ActiveNodeId);
    }

    [Fact]
    public void Clear_RemovesAllSelectionAndActiveNode()
    {
        var selection = new SelectionState();
        selection.AddToSelection(DocumentNodeId.New());
        selection.AddToSelection(DocumentNodeId.New(), makeActive: true);

        selection.Clear();

        Assert.True(selection.IsEmpty);
        Assert.Empty(selection.SelectedNodeIds);
        Assert.Null(selection.ActiveNodeId);
    }

    [Fact]
    public void RemoveFromSelection_WhenRemovingActiveNode_MovesActiveToLastSelected()
    {
        var selection = new SelectionState();
        var first = DocumentNodeId.New();
        var second = DocumentNodeId.New();
        var third = DocumentNodeId.New();

        selection.AddToSelection(first);
        selection.AddToSelection(second);
        selection.AddToSelection(third, makeActive: true);

        var removed = selection.RemoveFromSelection(third);

        Assert.True(removed);
        Assert.Equal(second, selection.ActiveNodeId);
        Assert.Equal(2, selection.SelectedNodeIds.Count);
    }

    [Fact]
    public void Operations_WithDefaultNodeId_ThrowArgumentException()
    {
        var selection = new SelectionState();

        Assert.Throws<ArgumentException>(() => selection.ReplaceSelection(default));
        Assert.Throws<ArgumentException>(() => selection.AddToSelection(default));
        Assert.Throws<ArgumentException>(() => selection.RemoveFromSelection(default));
        Assert.Throws<ArgumentException>(() => selection.ToggleSelection(default));
        Assert.Throws<ArgumentException>(() => selection.IsSelected(default));
    }

    [Fact]
    public void NodeOverloads_UseDocumentNodeId()
    {
        var selection = new SelectionState();
        var node = new Layer(DocumentNodeId.New(), "L", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        selection.AddToSelection(node, makeActive: true);

        Assert.True(selection.IsSelected(node.Id));
        Assert.Equal(node.Id, selection.ActiveNodeId);

        selection.ToggleSelection(node);

        Assert.False(selection.IsSelected(node.Id));
    }
}
