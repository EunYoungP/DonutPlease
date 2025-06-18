using UnityEngine;

public class FxOnCompleteUIInteraction : IFluxAction
{
    public readonly int interactionId;
    public FxOnCompleteUIInteraction(int interactionId)
    {
        this.interactionId = interactionId;
    }
}
