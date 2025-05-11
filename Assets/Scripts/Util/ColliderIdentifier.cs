using DonutPlease.Game.Character;
using UnityEngine;

public enum EColliderIdentifier : int
{
    None,
    GetDonut,
    TakeDonut,
    Cash,
    CasherPlace
}

public class ColliderIdentifier : MonoBehaviour
{
    [SerializeField]private EColliderIdentifier eColliderIdentifier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterBase characterBase = other.GetComponent<CharacterBase>();
            FluxSystem.DispatchColliderEnterEvent(characterBase, eColliderIdentifier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterBase characterBase = other.GetComponent<CharacterBase>();
            FluxSystem.DispatchColliderExitEvent(characterBase, eColliderIdentifier);
        }
    }
}
