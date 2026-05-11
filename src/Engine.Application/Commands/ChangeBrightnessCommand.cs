using Engine.Application.Commanding;
using Engine.Domain.Entities;

namespace Engine.Application.Commands;

public class ChangeBrightnessCommand : UndoableEditorCommand
{
    private readonly float _value;
    private float _previowsEffect;

    public ChangeBrightnessCommand(float value) : base("change.brightness.command")
    {
        _value = value;
    }

    public override void Execute(CommandContext context)
    {
        var layerId = context.Document.Selection.ActiveNodeId;
        if (layerId is null)
            return;

        var layer = context.Document.GetLayer(layerId.Value);

        var existentEffect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (existentEffect is null)
            throw new InvalidOperationException("No brightness effect do change");

        _previowsEffect = existentEffect.Intensity;
        existentEffect.SetIntensity(_value);
        context.RecordChange(new EffectParameterChangedChange(layerId.Value, existentEffect.Id));

    }

    public override void Undo(CommandContext context)
    {
        var layerId = context.Document.Selection.ActiveNodeId;
        if (layerId is null)
            return;

        var layer = context.Document.GetLayer(layerId.Value);

        var existentEffect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (existentEffect is null)
            throw new InvalidOperationException("No brightness effect to undo");

        existentEffect.SetIntensity(_previowsEffect);

        context.RecordChange(new EffectParameterChangedChange(layerId.Value, existentEffect.Id));

    }
}
