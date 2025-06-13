using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;


public class CharacterStockComponent : ComponentBase
{
    // Donut�� Trash ����� ��� ��Ƶ�
    public Dictionary<EItemType, List<Item>> Items { get; private set; } = new Dictionary<EItemType, List<Item>>();

    // Trash
    public Stack<GameObject> Trash { get; private set; } = new Stack<GameObject>();

    public void AddItem(Item item)
    {
        if (!Items.TryAdd(item.ItemType, new List<Item> { item }))
            Items[item.ItemType].Add(item);
    }

    public Item RemoveItem(EItemType itemType)
    {
        var items = GetItemByType(itemType);
        if (items.Count == 0)
        {
            Debug.LogWarning("No Items to remove.");
            return null;
        }

        var item = items[items.Count - 1];
        Items[itemType].Remove(item);

        return item;
    }

    public List<Item> GetItemByType(EItemType itemType)
    {
        return Items.TryGetValue(itemType, out var itemList) ? itemList : new List<Item>();
    }

    public bool CanGetItemType(EItemType itemType)
    {
        if (itemType == EItemType.Cash)
            return false;

        int totalItemCount = 0;
        foreach (var (type, itemList) in Items)
        {
            totalItemCount += itemList.Count;
            if ( 0 < itemList.Count)
            {
                if (itemType == EItemType.Donut
                 && GameManager.GetGameManager.Player.Character.Stat.DonutCapacity <= itemList.Count)
                    return false;

                if (type == itemType)
                    return true;
            }
        }

        if (totalItemCount == 0)
            return true;

        return false;
    }
}
