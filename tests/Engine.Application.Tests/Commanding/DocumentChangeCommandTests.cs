using Engine.Application.Commanding;
using Engine.Application.Commands;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Application.Tests.Commanding;

public sealed class DocumentChangeCommandTests
{
    [Fact]
    public void Commands_RecordSemanticDocumentChanges()
    {
        var nodeId = DocumentNodeId.New();
        var doc = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        var layer = new Layer(nodeId, "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        doc.AddNode(layer);
        var ctx = new CommandContext(doc);

        new RenameNodeCommand(nodeId, "Renamed").Execute(ctx);
        new SetLayerTransformCommand(nodeId, new LayerTransform(10, 20, 2, 2, 90)).Execute(ctx);
        new RotateLayerCommand(nodeId, 180).Execute(ctx);
        new SetNodeVisibilityCommand(nodeId, false).Execute(ctx);

        Assert.Contains(ctx.Changes, x => x is NodeRenamedChange);
        Assert.Contains(ctx.Changes, x => x is TransformChangedChange);
        Assert.Contains(ctx.Changes, x => x is VisibilityChangedChange);
        Assert.Equal(180, doc.GetLayer(nodeId).Transform.RotationDegrees);
    }
}
