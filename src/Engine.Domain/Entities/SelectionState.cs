using Engine.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace Engine.Domain.Entities;

/// <summary>
/// Estado de seleção de nós do documento, independente de UI e viewport.
/// </summary>
public sealed class SelectionState
{
    private readonly HashSet<DocumentNodeId> _selectedNodeIds = new();
    private readonly List<DocumentNodeId> _selectionOrder = new();
    private readonly ReadOnlyCollection<DocumentNodeId> _readOnlySelection;

    public SelectionState()
    {
        _readOnlySelection = _selectionOrder.AsReadOnly();
    }

    public IReadOnlyList<DocumentNodeId> SelectedNodeIds => _readOnlySelection;

    public DocumentNodeId? ActiveNodeId { get; private set; }

    public bool IsEmpty => _selectedNodeIds.Count == 0;

    public bool IsSelected(DocumentNodeId nodeId)
    {
        ValidateNodeId(nodeId);
        return _selectedNodeIds.Contains(nodeId);
    }

    public void ReplaceSelection(DocumentNodeId nodeId)
    {
        ValidateNodeId(nodeId);

        _selectedNodeIds.Clear();
        _selectionOrder.Clear();

        _selectedNodeIds.Add(nodeId);
        _selectionOrder.Add(nodeId);
        ActiveNodeId = nodeId;
    }

    public void ReplaceSelection(DocumentNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        ReplaceSelection(node.Id);
    }

    public bool AddToSelection(DocumentNodeId nodeId, bool makeActive = false)
    {
        ValidateNodeId(nodeId);

        if (!_selectedNodeIds.Add(nodeId))
        {
            if (makeActive)
            {
                ActiveNodeId = nodeId;
            }

            return false;
        }

        _selectionOrder.Add(nodeId);

        if (makeActive || ActiveNodeId is null)
        {
            ActiveNodeId = nodeId;
        }

        return true;
    }

    public bool AddToSelection(DocumentNode node, bool makeActive = false)
    {
        ArgumentNullException.ThrowIfNull(node);
        return AddToSelection(node.Id, makeActive);
    }

    public bool RemoveFromSelection(DocumentNodeId nodeId)
    {
        ValidateNodeId(nodeId);

        if (!_selectedNodeIds.Remove(nodeId))
        {
            return false;
        }

        _selectionOrder.Remove(nodeId);

        if (ActiveNodeId == nodeId)
        {
            ActiveNodeId = _selectionOrder.Count > 0 ? _selectionOrder[^1] : null;
        }

        return true;
    }

    public bool RemoveFromSelection(DocumentNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return RemoveFromSelection(node.Id);
    }

    public void Clear()
    {
        _selectedNodeIds.Clear();
        _selectionOrder.Clear();
        ActiveNodeId = null;
    }

    public bool ToggleSelection(DocumentNodeId nodeId, bool makeActiveWhenSelected = true)
    {
        ValidateNodeId(nodeId);

        if (_selectedNodeIds.Contains(nodeId))
        {
            return RemoveFromSelection(nodeId);
        }

        return AddToSelection(nodeId, makeActiveWhenSelected);
    }

    public bool ToggleSelection(DocumentNode node, bool makeActiveWhenSelected = true)
    {
        ArgumentNullException.ThrowIfNull(node);
        return ToggleSelection(node.Id, makeActiveWhenSelected);
    }

    private static void ValidateNodeId(DocumentNodeId nodeId)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }
    }
}
