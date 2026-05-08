using System.Collections.ObjectModel;

namespace Engine.Domain.Entities;

public sealed class LayerGroup : DocumentNode
{
    private readonly List<DocumentNode> _children = new();
    private readonly ReadOnlyCollection<DocumentNode> _readOnlyChildren;

    public LayerGroup(Guid id, string name, bool visibility = true)
        : base(id, name, visibility)
    {
        _readOnlyChildren = _children.AsReadOnly();
    }

    public IReadOnlyList<DocumentNode> Children => _readOnlyChildren;

    public void AddChild(DocumentNode node, int? index = null)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node == this)
        {
            throw new InvalidOperationException("A node cannot be added as a child of itself.");
        }

        if (ContainsDescendant(node))
        {
            throw new InvalidOperationException("A node cannot be added twice in the same hierarchy.");
        }

        if (IsAncestorOf(node))
        {
            throw new InvalidOperationException("Cannot add an ancestor as a child.");
        }

        if (node.Parent is not null)
        {
            throw new InvalidOperationException("Node already belongs to another parent.");
        }

        var targetIndex = index ?? _children.Count;
        if (targetIndex < 0 || targetIndex > _children.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be inside child range.");
        }

        _children.Insert(targetIndex, node);
        node.SetParent(this);
    }

    public bool RemoveChild(Guid nodeId)
    {
        var index = _children.FindIndex(x => x.Id == nodeId);
        if (index < 0)
        {
            return false;
        }

        var node = _children[index];
        _children.RemoveAt(index);
        node.SetParent(null);
        return true;
    }

    public void ReorderChild(Guid nodeId, int newIndex)
    {
        if (newIndex < 0 || newIndex >= _children.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newIndex), "Index must target an existing child slot.");
        }

        var currentIndex = _children.FindIndex(x => x.Id == nodeId);
        if (currentIndex < 0)
        {
            throw new KeyNotFoundException($"Child node with id '{nodeId}' was not found.");
        }

        var node = _children[currentIndex];
        _children.RemoveAt(currentIndex);
        _children.Insert(newIndex, node);
    }

    private bool ContainsDescendant(DocumentNode node) =>
        _children.Any(child => child.Id == node.Id || (child is LayerGroup group && group.ContainsDescendant(node)));

    private bool IsAncestorOf(DocumentNode node)
    {
        var current = Parent;
        while (current is not null)
        {
            if (current.Id == node.Id)
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }
}
