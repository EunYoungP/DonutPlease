using DonutPlease.Game.Character;
using UniRx;
using UnityEngine;

public class TrashCan : PropBase
{
    [SerializeField] private Transform _trashCanFrontPos;
    [SerializeField] private Transform _trashDropPos;

    public Transform TrashCanFrontPos => _trashCanFrontPos;
    public Transform TrashDropPos => _trashDropPos;

    private void OnEnable()
    {
        FluxSystem.ColliderTriggerActionStream.Subscribe(data =>
        {
            if (data is FxOnTriggerEnter fxTriggerEnter)
            {
                OnTriggerEnterAction(fxTriggerEnter.characterBase, fxTriggerEnter.colliderType);
            }

        }).AddTo(this);
    }

    private void OnTriggerEnterAction(CharacterBase character, EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.TakeTrash)
        {
            if (character is CharacterPlayer player)
            {
                player.RemoveFromTray(EItemType.Trash, _trashDropPos);
            }
        }
    }
}
