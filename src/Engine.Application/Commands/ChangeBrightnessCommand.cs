using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public class ChangeBrightnessCommand : UndoableEditorCommand
{
    private float _previowsIntensity;

    public ChangeBrightnessCommand(DocumentNodeId nodeId, float intensity) : base("change.brightness.command")
    {
        NodeId = nodeId;
        Intensity = intensity;
    }

    public DocumentNodeId NodeId { get; }
    public float Intensity { get; }

    public override void Execute(CommandContext context)
    {

        var layer = context.Document.GetLayer(NodeId);

        var existentEffect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (existentEffect is null)
            throw new InvalidOperationException("No brightness effect do change");

        _previowsIntensity = existentEffect.Intensity;
        existentEffect.SetIntensity(Intensity);
        context.RecordChange(new EffectParameterChangedChange(layer.Id, existentEffect.Id, nameof(BrightnessEffect), nameof(BrightnessEffect.Intensity)));

    }

    public override void Undo(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);

        var existentEffect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (existentEffect is null)
            throw new InvalidOperationException("No brightness effect to undo");

        existentEffect.SetIntensity(_previowsIntensity);

        context.RecordChange(new EffectParameterChangedChange(layer.Id, existentEffect.Id, nameof(BrightnessEffect), nameof(BrightnessEffect.Intensity)));

    }
}
