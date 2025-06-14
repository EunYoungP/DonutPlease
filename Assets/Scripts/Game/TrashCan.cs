using DG.Tweening;
using UnityEngine;
using UniRx;
using DonutPlease.Game.Character;

public class TrashCan : PropBase
{
    [SerializeField] private Transform _trashCanFrontPos;
    [SerializeField] private Transform _trashDropPos;

    public Transform TrashCanFrontPos => _trashCanFrontPos;
    public Transform TrashDropPos => _trashDropPos;

    private void OnEnable()
    {
        FluxSystem.ColliderEnterActionStream.Subscribe(data =>
        {
            OnTriggerEnterAction(data.Item1, data.Item2);

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
