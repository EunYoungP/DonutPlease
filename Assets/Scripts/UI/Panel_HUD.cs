using UnityEngine;
using UniRx;

public class Panel_HUD : MonoBehaviour
{
    [SerializeField] private ExpBar _expBar;
    [SerializeField] private TextImage _textImageCash;
    [SerializeField] private TextImage _textImageGem;

    private void Awake()
    {
        FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnUpdateCurrency updateCurrency)
                {
                    if (updateCurrency.currencyType == CurrencyType.Cash)
                        OnUpdateCash(updateCurrency.value);
                    else if (updateCurrency.currencyType == CurrencyType.Gem)
                        OnUpdateGem(updateCurrency.value);
                }
                
            }).AddTo(this);

        FluxSystem.ActionStream
        .Subscribe(data =>
        {
            if (data is OnUpdatePlayerGrowth updateGrowth)
            {
                UpdateGrowth(updateGrowth.level, updateGrowth.exp, updateGrowth.maxExp);
            }
        });
    }

    private void UpdateGrowth(int level, int curExp, int maxExp)
    {
        _expBar.SetLevel(level);
        _expBar.SetExp(curExp, maxExp); 
    }

    private void OnUpdateCash(int cash)
    {
        _textImageCash.SetText(cash);
    }

    private void OnUpdateGem(int gem)
    {
        _textImageGem.SetText(gem);
    }
}
