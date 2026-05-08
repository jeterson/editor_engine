namespace Engine.Application.Commanding;

/// <summary>
/// Executa comandos da engine e registra histórico básico de execução.
/// </summary>
public sealed class CommandDispatcher
{
    private readonly List<EditorCommand> _executedCommands = new();
    private readonly UndoRedoManager _undoRedoManager;

    public CommandDispatcher() : this(new UndoRedoManager())
    {
    }

    public CommandDispatcher(UndoRedoManager undoRedoManager)
    {
        _undoRedoManager = undoRedoManager ?? throw new ArgumentNullException(nameof(undoRedoManager));
    }

    public IReadOnlyList<EditorCommand> ExecutedCommands => _executedCommands;

    public UndoRedoManager UndoRedoManager => _undoRedoManager;

    public void Dispatch(EditorCommand command, CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(context);

        command.Execute(context);
        _executedCommands.Add(command);

        if (command is UndoableEditorCommand undoableCommand)
        {
            _undoRedoManager.RecordExecuted(undoableCommand);
        }
    }

    public bool Undo(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return _undoRedoManager.Undo(context);
    }

    public bool Redo(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return _undoRedoManager.Redo(context);
    }
}
