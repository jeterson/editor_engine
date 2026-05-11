using Engine.Application.Commanding;
using Engine.Domain.Entities;

namespace Engine.Application.Commands;

public class AddOrChangeBrightnessCommand : UndoableEditorCommand
{
    private readonly float _value;
    private EditorCommand? _command;

    public AddOrChangeBrightnessCommand(float value) : base("add.or.change.brightness")
    {
        _value = value;
    }

    public override void Execute(CommandContext context)
    {
        var layerId = context.Document.Selection.ActiveNodeId;
        if (layerId is null)
            return;

        var layer = context.Document.GetLayer(layerId.Value);

        var effect = layer.EffectStack.Effects.OfType<BrightnessEffect>().FirstOrDefault();

        if (effect is null)
            _command = new AddBrightnessCommand(_value);
        else
            _command = new ChangeBrightnessCommand(_value);

        _command.Execute(context);
    }

    public override void Undo(CommandContext context)
    {
        if (_command is UndoableEditorCommand command)
            command.Undo(context);
    }
}
