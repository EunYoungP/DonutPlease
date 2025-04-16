using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    [SerializeField]
    private InteractionType _interactionType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FluxSystem.Dispatch(new OnTriggerEnterInteractionUI(_interactionType));
        }
    }
}
