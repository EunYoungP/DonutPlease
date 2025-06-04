using UnityEngine;

public enum EItemType
{
    Donut,
    Trash,
    Cash,
}

public class Item : MonoBehaviour
{
    [SerializeField] private EItemType _itemType;
    public EItemType ItemType => _itemType;
}
