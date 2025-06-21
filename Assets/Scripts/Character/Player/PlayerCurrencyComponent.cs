using UniRx;
using UnityEngine;

public enum CurrencyType
{
    Cash,
    Gem,
}

public class PlayerCurrencyComponent : ComponentBase
{
    protected readonly CompositeDisposable Disposables = new CompositeDisposable();

    public ReactiveProperty<int> Cash { get; private set; } = new();
    public ReactiveProperty<int> Gem { get; private set; } = new();

    public void Initialize(PlayerData playerData)
    {
        FluxSystem.ActionStream.Subscribe(data =>
        {
            if (data is FxOnUpdateCurrency updateCurrency)
            {
                OnUpdateCurrency(updateCurrency.currencyType, updateCurrency.value);
            }
        }).AddTo(Disposables);

        Cash.Value = playerData.cash;
        Gem.Value = playerData.gem;
    }

    public void AddCash(int amount)
    {
        if (amount <= 0)
            return;

        Cash.Value += Mathf.FloorToInt(amount * GameManager.GetGameManager.Player.Character.Stat.ProfitGrowthRate);

        GameManager.GetGameManager.Data.UpgradePlayerCurrency(CurrencyType.Cash, Cash.Value);
        GameManager.GetGameManager.Audio.PlaySFX(AudioClipNames.pileCashString);
    }

    public void RemoveCash(int amount)
    {
        if (amount <= 0 || Cash.Value < amount)
            return;

        Cash.Value -= amount;
        GameManager.GetGameManager.Data.UpgradePlayerCurrency(CurrencyType.Cash, Cash.Value);
        GameManager.GetGameManager.Audio.PlaySFX(AudioClipNames.pileCashString);
    }

    public void AddGem(int amount)
    {
        if (amount <= 0)
            return;

        Gem.Value += amount;
    }

    public void RemoveGem(int amount)
    {
        if (amount <= 0 || Gem.Value < amount)
            return;

        Gem.Value -= amount;
    }

    public bool PayCash(int cash)
    {
        if (cash <= 0 || Cash.Value < cash)
            return false;

        RemoveCash(cash);
        return true;
    }

    public bool PayGem(int gem)
    {
        if (gem <= 0 || Gem.Value < gem)
            return false;

        RemoveGem(gem);
        return true;
    }

    private void OnUpdateCurrency(CurrencyType type, int value)
    {
        if (type == CurrencyType.Cash)
            AddCash(value);
        else if (type == CurrencyType.Gem)
            AddGem(value);
    }
}
