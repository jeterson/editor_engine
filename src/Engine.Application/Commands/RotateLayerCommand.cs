using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public sealed class RotateLayerCommand : UndoableEditorCommand
{
    private LayerTransform? _previousTransform;

    public RotateLayerCommand(DocumentNodeId nodeId, double rotationDegrees) : base("document.layer.rotate")
    {
        if (nodeId == default) throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        if (double.IsNaN(rotationDegrees) || double.IsInfinity(rotationDegrees)) throw new ArgumentOutOfRangeException(nameof(rotationDegrees));
        NodeId = nodeId;
        RotationDegrees = rotationDegrees;
    }

    public DocumentNodeId NodeId { get; }
    public double RotationDegrees { get; }

    public override void Execute(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);
        _previousTransform = layer.Transform;
        layer.SetTransform(layer.Transform with { RotationDegrees = RotationDegrees });
        context.RecordChange(new TransformChangedChange(NodeId));
    }

    public override void Undo(CommandContext context)
    {
        if (_previousTransform is null) throw new InvalidOperationException("Cannot undo rotation before execute.");
        context.Document.GetLayer(NodeId).SetTransform(_previousTransform.Value);
    }
}
