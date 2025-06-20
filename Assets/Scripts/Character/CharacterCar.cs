using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCar : CharacterBase
{
    [SerializeField] private CharacterCarController _controller;

    private float _itemHeight = 0.2f;

    public CharacterCarController Controller => _controller;
    private List<Item> _items = new List<Item>();

    public void Pay(int getDonutCount, CashPile cashPile)
    {
        cashPile.MakeCashInPile(this, getDonutCount);
    }

    public override void AddToTray(Transform child)
    {
        var item = child.GetComponent<Item>();

        if (item == null)
            return;

        Vector3 dest = transform.position + Vector3.up * _itemHeight;

        item.transform.DOJump(dest, 5, 1, 0.3f)
            .OnComplete(() =>
            {
                GameManager.GetGameManager.Audio.PlaySFX(AudioClipNames.pileDonutString);

                _items.Add(item);
                item.transform.SetParent(transform);
            });
    }

    public void AddToCharacter(Transform child)
    {
        
    }
}
