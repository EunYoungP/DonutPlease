using System.Collections.Generic;
using UnityEngine;

public class UIEmojiPool
{
    private readonly Queue<UIEmoji> _pool = new();
    private readonly Transform _parent;
    private readonly GameObject _prefab;

    public UIEmojiPool(GameObject prefab,int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);

            var uiEmoji = obj.GetComponent<UIEmoji>();
            _pool.Enqueue(uiEmoji);
        }
    }

    public UIEmoji Get(string emojiName)
    {
        UIEmoji uiEmoji;
        if (_pool.Count > 0)
        {
            uiEmoji = _pool.Dequeue();
        }
        else
        {
            uiEmoji = GameObject.Instantiate(_prefab, _parent).GetComponent<UIEmoji>();
        }

        uiEmoji.SetImage(emojiName);
        uiEmoji.gameObject.SetActive(true);
        return uiEmoji;
    }

    public void Release(UIEmoji uiEmoji)
    {
        uiEmoji.gameObject.SetActive(false);
        _pool.Enqueue(uiEmoji);
    }
}
