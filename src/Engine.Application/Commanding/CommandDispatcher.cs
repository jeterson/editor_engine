namespace Engine.Application.Commanding;

/// <summary>
/// Executa comandos da engine e registra histórico básico de execução.
/// </summary>
public sealed class CommandDispatcher
{
    private readonly List<EditorCommand> _executedCommands = new();

    public IReadOnlyList<EditorCommand> ExecutedCommands => _executedCommands;

    public void Dispatch(EditorCommand command, CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(context);

        command.Execute(context);
        _executedCommands.Add(command);
    }
}
