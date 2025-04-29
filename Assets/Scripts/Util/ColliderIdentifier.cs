using DonutPlease.Game.Character;
using UnityEngine;

public enum EColliderIdentifier : int
{
    None,
    Donut,
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
            ColliderIdentifier colliderIdentifier = other.GetComponent<ColliderIdentifier>();
            CharacterBase characterBase = other.GetComponent<CharacterBase>();
            FluxSystem.DispatchColliderEvent(characterBase, eColliderIdentifier);
        }
    }
}
