using UnityEngine;

public class FxOnUpdateCurrency : IFluxAction
{
    public readonly CurrencyType currencyType;
    public readonly int value;

    public FxOnUpdateCurrency(CurrencyType currencyType, int value)
    {
        this.currencyType = currencyType;
        this.value = value;
    }
}
