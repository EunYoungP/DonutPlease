
public class FxOnInteractionUICallback : IFluxAction
{
    public readonly int interactionId;

    public FxOnInteractionUICallback(int interactionId)
    {
        this.interactionId = interactionId;
    }
}
