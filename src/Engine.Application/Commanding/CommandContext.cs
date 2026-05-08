using Engine.Domain.Entities;

namespace Engine.Application.Commanding;

/// <summary>
/// Contexto de execução de comandos com acesso ao estado do domínio.
/// </summary>
public sealed class CommandContext
{
    public CommandContext(EditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        Document = document;
    }

    public EditorDocument Document { get; }

    public SelectionState Selection => Document.Selection;
}
