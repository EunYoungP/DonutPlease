using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    private static Dictionary<int, Store> _stores = new();

    public void Initialize()
    {
        // 1 Store »ý¼º
        AddOrUpdateStore(1);
    }

    public void AddOrUpdateStore(int storeId)
    {
        GameObject go = new GameObject($"Store_{storeId}");
        Store store = go.AddComponent<Store>();

        if (_stores.TryAdd(storeId, store))
            _stores[storeId] = store;
    }

    public Store GetStore(int storeId)
    {
        if (_stores.TryGetValue(storeId, out Store store))
        {
            // Store found
            Debug.Log($"Store found: {store}");
            return store;
        }
        else
        {
            // Store not found
            Debug.LogError($"Store with ID {storeId} not found.");
        }

        return null;
    }
}
