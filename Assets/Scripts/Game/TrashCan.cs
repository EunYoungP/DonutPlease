using DG.Tweening;
using UnityEngine;

public class TrashCan : PropBase
{
    [SerializeField] private Transform _trashCanFrontPos;
    [SerializeField] private Transform _trashDropPos;

    public Transform TrashCanFrontPos => _trashCanFrontPos;
    public Transform TrashDropPos => _trashDropPos;
}
