using UnityEngine;

public class OnUpdateCurrency
{
    public readonly CurrencyType currencyType;
    public readonly int value;

    public OnUpdateCurrency(CurrencyType currencyType, int value)
    {
        this.currencyType = currencyType;
        this.value = value;
    }
}
