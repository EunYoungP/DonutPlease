using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolSystem : IDisposable
{
    public GameObjectPool GameObjectPool { get; private set; }

    public void Initialize()
    {
        GameObjectPool = new GameObjectPool(new GameObject(), 10, null);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
