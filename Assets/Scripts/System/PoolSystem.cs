using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolSystem : IDisposable
{
    public GameObjectPool GameObjectPool { get; private set; }
    public AudioSourcePool AudioSourcePool { get; private set; }

    public void Initialize()
    {
        GameObjectPool = new GameObjectPool(new GameObject(), 10, null);
        AudioSourcePool = new AudioSourcePool(10, GameManager.GetGameManager.Audio.transform);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
