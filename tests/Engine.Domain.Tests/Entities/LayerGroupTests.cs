using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class LayerGroupTests
{
    [Fact]
    public void AddRemoveAndReorderChildren_PreservesOrder()
    {
        var group = new LayerGroup(DocumentNodeId.New(), "Root");
        var first = new Layer(DocumentNodeId.New(), "A", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var second = new Layer(DocumentNodeId.New(), "B", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        group.AddChild(first);
        group.AddChild(second);
        group.ReorderChild(second.Id, 0);

        Assert.Equal(second.Id, group.Children[0].Id);
        Assert.Equal(first.Id, group.Children[1].Id);

        var removed = group.RemoveChild(first.Id);

        Assert.True(removed);
        Assert.Single(group.Children);
    }

    [Fact]
    public void AddChild_WithNestedGroups_SupportsHierarchy()
    {
        var root = new LayerGroup(DocumentNodeId.New(), "Root");
        var childGroup = new LayerGroup(DocumentNodeId.New(), "Child");
        var leaf = new Layer(DocumentNodeId.New(), "Leaf", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        root.AddChild(childGroup);
        childGroup.AddChild(leaf);

        Assert.Same(root, childGroup.Parent);
        Assert.Same(childGroup, leaf.Parent);
    }

    [Fact]
    public void AddChild_PreventsInvalidStateAndCycles()
    {
        var root = new LayerGroup(DocumentNodeId.New(), "Root");
        var child = new LayerGroup(DocumentNodeId.New(), "Child");

        root.AddChild(child);

        Assert.Throws<InvalidOperationException>(() => child.AddChild(root));
        Assert.Throws<InvalidOperationException>(() => root.AddChild(child));
    }
}
