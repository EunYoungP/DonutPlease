using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;

public class Panel_HUD : UIBehaviour
{
    [SerializeField] private ExpBar _expBar;
    [SerializeField] private TextImage _textImageCash;
    [SerializeField] private TextImage _textImageGem;


    protected override void OnEnable()
    {
        GameManager.GetGameManager.Player.Currency.Cash.Subscribe(cash => OnUpdateCash(cash));
        GameManager.GetGameManager.Player.Currency.Gem.Subscribe(gem => OnUpdateGem(gem));

        var maxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value);
        var prevMaxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value - 1);
        GameManager.GetGameManager.Player.Growth.Exp.Subscribe(exp => _expBar.SetExp(exp - prevMaxExp, maxExp - prevMaxExp));
        GameManager.GetGameManager.Player.Growth.Level.Subscribe(level => _expBar.SetLevel(level));
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
