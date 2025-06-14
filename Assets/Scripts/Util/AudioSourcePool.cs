using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool
{
    private readonly Queue<AudioSource> _pool = new();
    private readonly Transform _parent;
    private readonly AudioSource _prefab;

    public AudioSourcePool(AudioSource prefab, int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var source = GameObject.Instantiate(_prefab, _parent);
            source.gameObject.SetActive(false);
            _pool.Enqueue(source);
        }
    }

    public AudioSource Get()
    {
        AudioSource source;
        if (_pool.Count > 0)
        {
            source = _pool.Dequeue();
        }
        else
        {
            source = GameObject.Instantiate(_prefab, _parent);
        }

        source.gameObject.SetActive(true);
        return source;
    }

    public void Release(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        _pool.Enqueue(source);
    }
}

