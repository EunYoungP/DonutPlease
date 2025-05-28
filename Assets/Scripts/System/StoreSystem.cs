using DonutPlease.Game.Character;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject WorkerPrefab;
    [SerializeField] private Vector3 WorkerStartPos;

    // for test
    [SerializeField] public Counter Counter;
    [SerializeField] public TrashCan TrashCan;

    private static Dictionary<int, Store> _stores = new();

    public void Initialize()
    {
        // 1 Store 积己
        AddStore(1);
    }

    public CharacterWorker CreateWorker()
    {
        // 况目 积己
        GameObject workerObj = Instantiate(WorkerPrefab, WorkerStartPos, Quaternion.identity);
        return workerObj.GetComponent<CharacterWorker>();
    }

    public void AddStore(int storeId)
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
