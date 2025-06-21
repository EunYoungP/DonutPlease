using DonutPlease.Game.Character;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject WorkerPrefab;
    [SerializeField] private Vector3 WorkerStartPos;

    // for test
    [SerializeField] public TrashCan TrashCan;


    public int CurStoreId { get; private set; }
    public Store CurStore => GetStore(CurStoreId);
    private static Dictionary<int, Store> _stores = new();

    private void Awake()
    {
        
    }

    public void Initialize()
    {
        int storeId = 1;

        // Store 생성
        AddStore(storeId);
        CurStoreId = storeId;

        foreach (var (id, store) in _stores)
        {
            store.Stat.HiredCount.Subscribe(hiredCount =>
            {
                // 워커 수가 변경될 때마다 StoreStatComponent의 HiredCount 업데이트
                for(int i = store.Workers.Count; i < hiredCount; i++) 
                    CreateWorker(storeId);
            });
        }
    }

    public CharacterWorker CreateWorker(int storeId)
    {
        // 워커 생성
        GameObject workerObj = Instantiate(WorkerPrefab, WorkerStartPos, Quaternion.identity);
        var worker = workerObj.GetComponent<CharacterWorker>();
        var store = GetStore(storeId);
        store.CreateWorker(worker);

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
