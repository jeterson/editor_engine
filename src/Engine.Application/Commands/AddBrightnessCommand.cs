using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public class AddBrightnessCommand : UndoableEditorCommand
{
    private readonly float _value;
    private EffectId? referenceEffectId;

    public AddBrightnessCommand(float value) : base("add.brightness.command")
    {
        _value = value;
    }

    public override void Execute(CommandContext context)
    {
        var layerId = context.Document.Selection.ActiveNodeId;
        if (layerId is null)
            return;

        var layer = context.Document.GetLayer(layerId.Value);
        referenceEffectId = EffectId.New();
        var effect = new BrightnessEffect(EffectId.New(), isEnabled: true, _value);
        layer.EffectStack.Add(effect);

        context.RecordChange(new EffectChangedChange(layerId.Value, effect.Id, nameof(BrightnessEffect)));
    }

    public override void Undo(CommandContext context)
    {
        var layerId = context.Document.Selection.ActiveNodeId;
        if (layerId is null)
            return;

        var layer = context.Document.GetLayer(layerId.Value);
        if (referenceEffectId is null)
            throw new InvalidOperationException("The are no effect with to undo");

        layer.EffectStack.Remove(referenceEffectId.Value);
        context.RecordChange(new EffectChangedChange(layerId.Value, referenceEffectId.Value, nameof(BrightnessEffect)));
    }
}
