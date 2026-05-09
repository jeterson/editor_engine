using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class SetLayerTransformCommand : UndoableEditorCommand
{
    private LayerTransform? _previousTransform;

    public SetLayerTransformCommand(DocumentNodeId nodeId, LayerTransform transform) : base("document.layer.transform")
    {
        if (nodeId == default) throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        NodeId = nodeId;
        Transform = transform;
    }

    public DocumentNodeId NodeId { get; }
    public LayerTransform Transform { get; }

    public override void Execute(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);
        _previousTransform = layer.Transform;
        layer.SetTransform(Transform);
        context.RecordChange(new TransformChangedChange(NodeId));
    }

    public override void Undo(CommandContext context)
    {
        if (_previousTransform is null) throw new InvalidOperationException("Cannot undo transform before execute.");
        context.Document.GetLayer(NodeId).SetTransform(_previousTransform.Value);
    }
}
