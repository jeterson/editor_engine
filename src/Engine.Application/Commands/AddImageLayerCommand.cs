using Engine.Application.Commanding;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.Application.Commands;

public class AddImageLayerCommand : AddNodeCommand<Layer>
{
    public AddImageLayerCommand(string name, AssetReference assetReference) : base(CreateLayer(name, assetReference))
    {
    }

    public override void Execute(CommandContext context)
    {
        base.Execute(context);
        context.Document.Selection.AddToSelection(Node);
    }

    private static Layer CreateLayer(
       string name,
       AssetReference assetReference)
    {
        return new Layer(
            DocumentNodeId.New(),
            name,
            true,
            LayerTransform.Identity,
            Opacity.Opaque,
            BlendMode.Normal,
            assetReference);
    }
}
