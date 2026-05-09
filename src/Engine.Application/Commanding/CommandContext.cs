using Engine.Domain.Entities;

namespace Engine.Application.Commanding;

/// <summary>
/// Contexto de execução de comandos com acesso ao estado do domínio.
/// </summary>
public sealed class CommandContext
{
    private readonly List<DocumentChange> _changes = new();
    public CommandContext(EditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        Document = document;
    }

    public EditorDocument Document { get; }

    public SelectionState Selection => Document.Selection;

    public IReadOnlyList<DocumentChange> Changes => _changes;

    public void RecordChange(DocumentChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        _changes.Add(change);
    }

    public void ClearChanges() => _changes.Clear();
}
