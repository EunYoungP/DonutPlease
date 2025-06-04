using UnityEngine;

public enum EItemType : int
{
    Donut = 0,
    Trash,
    Cash,
}

public class Item : MonoBehaviour
{
    [SerializeField] private EItemType _itemType;
    public EItemType ItemType => _itemType;
}
