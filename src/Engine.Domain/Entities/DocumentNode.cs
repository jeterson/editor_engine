namespace Engine.Domain.Entities;

/// <summary>
/// Nó estrutural base do documento.
/// </summary>
public abstract class DocumentNode
{
    private DocumentNode? _parent;

    protected DocumentNode(Guid id, string name, bool visibility)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Node id must be non-empty.", nameof(id));
        }

        ValidateName(name);

        Id = id;
        Name = name;
        Visibility = visibility;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public bool Visibility { get; private set; }

    public DocumentNode? Parent => _parent;

    public void Rename(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void SetVisibility(bool visibility) => Visibility = visibility;

    internal void SetParent(DocumentNode? parent) => _parent = parent;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Node name must be provided.", nameof(name));
        }
    }
}
