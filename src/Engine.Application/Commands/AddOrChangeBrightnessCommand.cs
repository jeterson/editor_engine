using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public class AddOrChangeBrightnessCommand : UndoableEditorCommand
{
    private EditorCommand? _command;

    public AddOrChangeBrightnessCommand(DocumentNodeId nodeId, float Intensity) : base("add.or.change.brightness")
    {
        NodeId = nodeId;
        this.Intensity = Intensity;
    }

    public DocumentNodeId NodeId { get; }
    public float Intensity { get; }

    public override void Execute(CommandContext context)
    {
        var layer = context.Document.GetLayer(NodeId);

        var effect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (effect is null)
            _command = new AddBrightnessCommand(NodeId, Intensity);
        else
            _command = new ChangeBrightnessCommand(NodeId, Intensity);

        _command.Execute(context);
    }

    public override void Undo(CommandContext context)
    {
        if (_command is UndoableEditorCommand command)
            command.Undo(context);
    }
}
