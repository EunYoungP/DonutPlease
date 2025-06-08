using UnityEngine;
using UniRx;
using DonutPlease.Game.Character;
using System;

public class PlayerCurrencyComponent : ComponentBase
{
    public int Cash { get; private set; }
    public int Gem { get; private set; }

    public void Initialize()
    {
        FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnGetItem getDonut)
                {
                    if (getDonut.character is CharacterPlayer player)
                    {
                        OnGetCash(getDonut);
                    }
                }
            }).AddTo(Disposables);
    }

    public void AddCash(int amount)
    {
        if (amount <= 0)
            return;

        Cash += amount;
    }

    public void RemoveCash(int amount)
    {
        if (amount < 0 || Cash < amount)
            return;

        Cash -= amount;
    }

    #region OnEvent

    private void OnGetCash(OnGetItem onGetCash)
    {
        var item = onGetCash.item.GetComponent<Item>();
        if (item.ItemType != EItemType.Cash)
            return;

        AddCash(item.RewardCash);
    }

    #endregion
}
