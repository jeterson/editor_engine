using System.Collections.ObjectModel;
using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Raiz de agregado que representa o estado do documento editável.
/// </summary>
public sealed class EditorDocument
{
    private readonly List<DocumentNode> _nodes = new();
    private readonly ReadOnlyCollection<DocumentNode> _readOnlyNodes;

    public EditorDocument(DocumentId id, CanvasSize canvasSize)
    {
        if (id == default)
        {
            throw new ArgumentException("Document id must be non-default.", nameof(id));
        }

        Id = id;
        CanvasSize = canvasSize;
        _readOnlyNodes = _nodes.AsReadOnly();
    }

    public DocumentId Id { get; }

    public CanvasSize CanvasSize { get; }

    public LayerId AddLayer(string name, AssetReference assetReference, bool visibility = true)
    {
        var layer = new Layer(LayerId.New(), name, visibility, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, assetReference);
        AddNode(layer);
        return layer.Id;
    }

    public Guid AddLayerGroup(string name, bool visibility = true)
    {
        var group = new LayerGroup(Guid.NewGuid(), name, visibility);
        AddNode(group);
        return group.Id;
    }

    public void AddNode(DocumentNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Parent is not null)
        {
            throw new InvalidOperationException("Root nodes must not have a parent.");
        }

        if (_nodes.Any(existing => existing.Id == node.Id))
        {
            throw new InvalidOperationException($"Node with id '{node.Id}' already exists.");
        }

        _nodes.Add(node);
    }

    public bool RemoveLayer(LayerId layerId)
    {
        if (layerId == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(layerId));
        }

        var index = _nodes.FindIndex(x => x is Layer layer && layer.Id == layerId);
        if (index < 0)
        {
            return false;
        }

        _nodes.RemoveAt(index);
        return true;
    }

    public bool RemoveNode(Guid nodeId)
    {
        var index = _nodes.FindIndex(x => x.Id == nodeId);
        if (index < 0)
        {
            return false;
        }

        _nodes.RemoveAt(index);
        return true;
    }

    public Layer GetLayer(LayerId layerId)
    {
        if (layerId == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(layerId));
        }

        var layer = _nodes.OfType<Layer>().FirstOrDefault(x => x.Id == layerId);
        if (layer is null)
        {
            throw new KeyNotFoundException($"Layer with id '{layerId}' was not found.");
        }

        return layer;
    }

    public IReadOnlyList<Layer> EnumerateLayers() => _nodes.OfType<Layer>().ToArray();

    public IReadOnlyList<DocumentNode> EnumerateNodes() => _readOnlyNodes;
}
