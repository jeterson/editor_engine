using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class LayerGroupTests
{
    [Fact]
    public void AddRemoveAndReorderChildren_PreservesOrder()
    {
        var group = new LayerGroup(Guid.NewGuid(), "Root");
        var first = new Layer(LayerId.New(), "A", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var second = new Layer(LayerId.New(), "B", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        group.AddChild(first);
        group.AddChild(second);
        group.ReorderChild(second.Id.Value, 0);

        Assert.Equal(second.Id.Value, group.Children[0].Id);
        Assert.Equal(first.Id.Value, group.Children[1].Id);

        var removed = group.RemoveChild(first.Id.Value);

        Assert.True(removed);
        Assert.Single(group.Children);
    }

    [Fact]
    public void AddChild_WithNestedGroups_SupportsHierarchy()
    {
        var root = new LayerGroup(Guid.NewGuid(), "Root");
        var childGroup = new LayerGroup(Guid.NewGuid(), "Child");
        var leaf = new Layer(LayerId.New(), "Leaf", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        root.AddChild(childGroup);
        childGroup.AddChild(leaf);

        Assert.Same(root, childGroup.Parent);
        Assert.Same(childGroup, leaf.Parent);
    }

    [Fact]
    public void AddChild_PreventsInvalidStateAndCycles()
    {
        var root = new LayerGroup(Guid.NewGuid(), "Root");
        var child = new LayerGroup(Guid.NewGuid(), "Child");

        root.AddChild(child);

        Assert.Throws<InvalidOperationException>(() => child.AddChild(root));
        Assert.Throws<InvalidOperationException>(() => root.AddChild(child));
    }
}
