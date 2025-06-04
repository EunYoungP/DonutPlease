
public class OnTriggerEnterInteractionUI
{
    public readonly int interactionId;
    public readonly InteractionType interactionType;
    public readonly int nextInerationId = -1;

    public OnTriggerEnterInteractionUI(int interactionId, InteractionType interactionType, int nextInerationId)
    {
        this.interactionId = interactionId;
        this.interactionType = interactionType;
        this.nextInerationId = nextInerationId;
    }
}
