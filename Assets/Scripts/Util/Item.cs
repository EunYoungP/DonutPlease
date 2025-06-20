using UnityEngine;

public enum EItemType : int
{
    Donut = 0,
    Trash,
    Cash,
    DonutBox,
}

public class Item : MonoBehaviour
{
    [SerializeField] private EItemType _itemType;
    [SerializeField] private int _reward;

    public EItemType ItemType => _itemType;
    public int RewardCash => _reward;
}
