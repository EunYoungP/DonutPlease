using UnityEngine;

public class FxOnCompleteUIInteraction : IFluxAction
{
    public readonly int interactionId;
    public readonly Transform uiInteractionTransform;
    public FxOnCompleteUIInteraction(int interactionId, Transform uiInteractionTransform)
    {
        this.interactionId = interactionId;
        this.uiInteractionTransform = uiInteractionTransform;
    }
}
