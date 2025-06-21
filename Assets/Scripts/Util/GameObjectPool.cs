using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool 
{
    private readonly Queue<GameObject> _pools = new();

    private GameObject _prefab;
    private Transform _parent;

    public GameObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);

            _pools.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (_pools.Count > 0)
        {
            var obj = _pools.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var obj = Object.Instantiate(_prefab, _parent);
            return obj;
        }
    }

    public void Release(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        _pools.Enqueue(obj);
    }
}
