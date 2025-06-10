using UnityEngine;
using UniRx;
using DonutPlease.Game.Character;
using System;

public enum CurrencyType
{
    Cash,
    Gem,
}

public class PlayerCurrencyComponent : ComponentBase
{
    public int Cash { get; private set; }
    public int Gem { get; private set; }

    public void Initialize(PlayerData playerData)
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

        Cash = playerData.cash;
        Gem = playerData.gem;
    }

    public void AddCash(int amount)
    {
        if (amount < 0)
            return;

        Cash += amount;

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
    }

    public void RemoveCash(int amount)
    {
        if (amount < 0 || Cash < amount)
            return;

        Cash -= amount;

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
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
