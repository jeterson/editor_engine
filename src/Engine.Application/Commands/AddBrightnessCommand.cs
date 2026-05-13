using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public class AddBrightnessCommand : UndoableEditorCommand
{
    private EffectId? referenceEffectId;

    public AddBrightnessCommand(DocumentNodeId nodeId, float intensity) : base("add.brightness.command")
    {
        NodeId = nodeId;
        Intensity = intensity;
    }

    public DocumentNodeId NodeId { get; }
    public float Intensity { get; }

    public override void Execute(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);
        referenceEffectId = EffectId.New();
        var effect = new BrightnessEffect(EffectId.New(), isEnabled: true, Intensity);
        layer.EffectStack.Add(effect);

        context.RecordChange(new EffectChangedChange(layer.Id, effect.Id, nameof(BrightnessEffect)));
    }

    public override void Undo(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);
        if (referenceEffectId is null)
            throw new InvalidOperationException("The are no effect with to undo");

        layer.EffectStack.Remove(referenceEffectId.Value);
        context.RecordChange(new EffectChangedChange(layer.Id, referenceEffectId.Value, nameof(BrightnessEffect)));
    }
}
