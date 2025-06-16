using DonutPlease.Game.Character;
using UnityEngine;

public enum EColliderIdentifier : int
{
    None,
    GetDonut,
    TakeDonut,
    Cash,
    CasherPlace,
    GetTrash,
    TakeTrash,
    InHR,
    InUpgrade,
}

public class ColliderIdentifier : MonoBehaviour
{
    [SerializeField]private EColliderIdentifier eColliderIdentifier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterBase characterBase = other.GetComponent<CharacterBase>();
            FluxSystem.DispatchColliderTriggerEvent(new FxOnTriggerEnter(characterBase, eColliderIdentifier));

            Debug.Log("Player Entered Collider");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterBase characterBase = other.GetComponent<CharacterBase>();
            FluxSystem.DispatchColliderTriggerEvent(new FxOnTriggerExit(characterBase, eColliderIdentifier));

            Debug.Log("Player Exited Collider");
        }
    }
}
