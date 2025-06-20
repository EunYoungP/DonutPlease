using DonutPlease.Game.Character;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject WorkerPrefab;
    [SerializeField] private Vector3 WorkerStartPos;

    // for test
    [SerializeField] public TrashCan TrashCan;


    public int CurStoreId { get; private set; }
    public Store CurStore => GetStore(CurStoreId);
    private static Dictionary<int, Store> _stores = new();

    public void Initialize()
    {
        int storeId = 1;

        // Store 积己
        AddStore(storeId);
        CurStoreId = storeId;
    }

    public CharacterWorker CreateWorker()
    {
        // 况目 积己
        GameObject workerObj = Instantiate(WorkerPrefab, WorkerStartPos, Quaternion.identity);
        var worker = workerObj.GetComponent<CharacterWorker>();
        CurStore.CreateWorker(worker);

        return worker;
    }

    public void AddStore(int storeId)
    {
        GameObject go = new GameObject($"Store_{storeId}");
        go.transform.SetParent(GameManager.GetGameManager.Store.transform);
        Store store = go.AddComponent<Store>();

        if (_stores.TryAdd(storeId, store))
            _stores[storeId] = store;

        store.Initialize();
    }

    public Store GetStore(int storeId)
    {
        if (_stores.TryGetValue(storeId, out Store store))
        {
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
