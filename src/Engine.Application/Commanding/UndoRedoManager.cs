namespace Engine.Application.Commanding;

/// <summary>
/// Gerencia histórico linear de comandos reversíveis usando pilhas de undo/redo.
/// </summary>
public sealed class UndoRedoManager
{
    private readonly Stack<UndoableEditorCommand> _undoStack = new();
    private readonly Stack<UndoableEditorCommand> _redoStack = new();

    public int UndoCount => _undoStack.Count;

    public int RedoCount => _redoStack.Count;

    public bool CanUndo => _undoStack.Count > 0;

    public bool CanRedo => _redoStack.Count > 0;

    public void RecordExecuted(UndoableEditorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public bool Undo(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (_undoStack.Count == 0)
        {
            return false;
        }

        var command = _undoStack.Pop();
        command.Undo(context);
        _redoStack.Push(command);
        return true;
    }

    public bool Redo(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (_redoStack.Count == 0)
        {
            return false;
        }

        var command = _redoStack.Pop();
        command.Execute(context);
        _undoStack.Push(command);
        return true;
    }
}
