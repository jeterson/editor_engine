namespace Engine.Application.Commanding;

/// <summary>
/// Representa uma intenção semântica de edição independente de UI/renderer.
/// </summary>
public abstract class EditorCommand
{
    protected EditorCommand(string semanticId)
    {
        if (string.IsNullOrWhiteSpace(semanticId))
        {
            throw new ArgumentException("Semantic id must be provided.", nameof(semanticId));
        }

        SemanticId = semanticId;
    }

    public string SemanticId { get; }

    public abstract void Execute(CommandContext context);
}

/// <summary>
/// Representa um comando de edição que pode reverter sua própria execução.
/// </summary>
public abstract class UndoableEditorCommand : EditorCommand
{
    protected UndoableEditorCommand(string semanticId) : base(semanticId)
    {
    }

    public abstract void Undo(CommandContext context);
}
