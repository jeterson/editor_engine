using System.Collections.ObjectModel;
using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Raiz de agregado que representa o estado do documento editável.
/// </summary>
public sealed class EditorDocument
{
    private readonly List<Layer> _layers = new();
    private readonly ReadOnlyCollection<Layer> _readOnlyLayers;

    public EditorDocument(DocumentId id, CanvasSize canvasSize)
    {
        if (id == default)
        {
            throw new ArgumentException("Document id must be non-default.", nameof(id));
        }

        Id = id;
        CanvasSize = canvasSize;
        _readOnlyLayers = _layers.AsReadOnly();
    }

    public DocumentId Id { get; }

    public CanvasSize CanvasSize { get; }

    public LayerId AddLayer(string name, bool visibility = true)
    {
        var layer = new Layer(LayerId.New(), name, visibility);
        _layers.Add(layer);
        return layer.Id;
    }

    public void AddLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (_layers.Any(existing => existing.Id == layer.Id))
        {
            throw new InvalidOperationException($"Layer with id '{layer.Id}' already exists.");
        }

        _layers.Add(layer);
    }

    public bool RemoveLayer(LayerId layerId)
    {
        if (layerId == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(layerId));
        }

        var index = _layers.FindIndex(x => x.Id == layerId);
        if (index < 0)
        {
            return false;
        }

        _layers.RemoveAt(index);
        return true;
    }

    public Layer GetLayer(LayerId layerId)
    {
        if (layerId == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(layerId));
        }

        var layer = _layers.FirstOrDefault(x => x.Id == layerId);
        if (layer is null)
        {
            throw new KeyNotFoundException($"Layer with id '{layerId}' was not found.");
        }

        return layer;
    }

    public IReadOnlyList<Layer> EnumerateLayers() => _readOnlyLayers;
}
