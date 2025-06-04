using UnityEngine;
using UniRx;
using NUnit.Framework;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class CharacterStockComponent 
{
    // Donut과 Trash 여기로 모두 모아둠
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
            return true;

        int totalItemCount = 0;
        foreach (var (type, itemList) in Items)
        {
            totalItemCount += itemList.Count;
            if (itemList.Count > 0)
            {
                if (type == itemType)
                    return true;
            }
        }

        if (totalItemCount == 0)
            return true;

        return false;
    }
}
