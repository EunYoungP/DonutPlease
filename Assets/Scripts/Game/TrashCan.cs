using UnityEngine;

public class TrashCan : PropBase
{
    [SerializeField] private Transform _trashCanFrontPos;

    public Transform TrashCanFrontPos => _trashCanFrontPos;
}
