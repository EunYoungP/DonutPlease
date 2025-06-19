using System;
using UnityEngine;

public class PoolSystem : IDisposable
{
    public GameObjectPool GameObjectPool { get; private set; }
    public AudioSourcePool AudioSourcePool { get; private set; }


    public UIEmojiPool UIEmojiPool { get; private set; }


    public void Initialize()
    {
        var Resource = GameManager.GetGameManager.Resource;

        GameObjectPool = new GameObjectPool(new GameObject(), 10, null);
        AudioSourcePool = new AudioSourcePool(10, GameManager.GetGameManager.Audio.transform);
        UIEmojiPool = new UIEmojiPool(Resource.GetAsset(Resource.GetUIPrefabPath("UIEmoji")), 10, GameManager.GetGameManager._canvas.transform);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
