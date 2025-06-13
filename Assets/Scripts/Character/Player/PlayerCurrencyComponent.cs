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
    protected readonly CompositeDisposable Disposables = new CompositeDisposable();

    public int Cash { get; private set; } = 0;
    public int Gem { get; private set; } = 0;

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

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Gem, Gem));
    }

    public void AddCash(int amount)
    {
        if (amount <= 0)
            return;

        Cash += Mathf.FloorToInt(amount * GameManager.GetGameManager.Player.Character.Stat.ProfitGrowthRate);

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
    }

    public void RemoveCash(int amount)
    {
        if (amount <= 0 || Cash < amount)
            return;

        Cash -= amount;

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
    }

    public void AddGem(int amount)
    {
        if (amount <= 0)
            return;

        Gem += amount;

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Gem, Gem));
    }

    public void RemoveGem(int amount)
    {
        if (amount <= 0 || Gem < amount)
            return;

        Gem -= amount;

        FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Gem, Gem));
    }

    public bool PayCash(int cash)
    {
        if (cash <= 0 || Cash < cash)
            return false;

        RemoveCash(cash);
        return true;
    }

    public bool PayGem(int gem)
    {
        if (gem <= 0 || Gem < gem)
            return false;

        RemoveGem(gem);
        return true;
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
