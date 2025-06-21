using DG.Tweening;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class Panel_HUD : UIBehaviour
{
    [SerializeField] private ExpBar _expBar;
    [SerializeField] private TextImage _textImageCash;
    [SerializeField] private TextImage _textImageGem;
    [SerializeField] private GameObject _expEffect;

    private GameManager _gameMng => GameManager.GetGameManager;
    private Canvas _canvas;
    private RectTransform _rectCanvas;

    protected override void OnEnable()
    {
        _canvas = GameManager.GetGameManager._canvas;
        _rectCanvas = _canvas.GetComponent<RectTransform>();

        GameManager.GetGameManager.Player.Currency.Cash.Subscribe(cash => OnUpdateCash(cash));
        GameManager.GetGameManager.Player.Currency.Gem.Subscribe(gem => OnUpdateGem(gem));

        //GameManager.GetGameManager.Player.Growth.Exp.Subscribe(exp => OnUpdateExp(exp));
        //GameManager.GetGameManager.Player.Growth.Level.Subscribe(level => OnUpdateLevel(level));

        FluxSystem.ActionStream.Subscribe(data =>
        {
            if (data is FxOnCompleteUIInteraction action)
            {
                OnCompleteUIInteraction(action.interactionId, action.uiInteractionTransform);
            }
        });

        Initialize();
    }

    private void Initialize()
    {
        UpdateLevel();
        UpdateExp();
    }


    private void OnUpdateCash(int cash)
    {
        _textImageCash.SetText(cash);
    }

    private void OnUpdateGem(int gem)
    {
        _textImageGem.SetText(gem);
    }

    private void UpdateLevel()
    {
        var level = _gameMng.Player.Growth.Level.Value;
        _expBar.SetLevel(level);
    }

    private void UpdateExp()
    {
        var curExp = GameManager.GetGameManager.Player.Growth.Exp.Value;
        var maxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value);
        var prevMaxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value - 1);

        _expBar.SetExp(curExp - prevMaxExp, maxExp - prevMaxExp);
    }

    private void OnCompleteUIInteraction(int interactionId, Transform startTransform)
    {
        var followCamera = GameManager.GetGameManager.Player.Character.Camera.MainCamera;
        var characterPos = GameManager.GetGameManager.Player.Character.gameObject.transform.position;
        var cameraPos = GameManager.GetGameManager.Player.Character.Camera.transform.position;

        var startPos = followCamera.WorldToScreenPoint(startTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectCanvas, startPos, _canvas.worldCamera, localPoint: out var midLocalPos);

        _expEffect.transform.position = startPos;
        _expEffect.SetActive(true);

        _expEffect.transform.localScale = Vector3.one * 0.7f;

        DOTween.Sequence().Append(_expEffect.transform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack))
           .Append(_expEffect.transform.DOScale(1.0f, 0.2f).SetEase(Ease.InQuad))
           .Play().SetAutoKill();

        _expEffect.transform
            .DOMove(_expBar.LevelObj.transform.position, 1.0f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
        {
            StartCoroutine(CoLerpExpCount());

            _expEffect.SetActive(false);
            _expEffect.transform.position = _expBar.LevelObj.transform.position;
        });
    }

    private IEnumerator CoLerpExpCount()
    {
        int curLevel = GameManager.GetGameManager.Player.Growth.Level.Value;
        int curExp = GameManager.GetGameManager.Player.Growth.Exp.Value;

        int barExp = _expBar.Exp;
        int barLevel = _expBar.Level;

        while (barLevel <= curLevel)
        {
            var curMaxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(barLevel);
            var prevMaxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(barLevel - 1);
            var maxExp = curMaxExp - prevMaxExp;
            var targetExp = barLevel != curLevel? maxExp : curExp - prevMaxExp;

            yield return DOTween.To(() => barExp, x =>
            {
                _expBar.SetExp(x, maxExp);
            }, targetExp, 1f).SetEase(Ease.OutCubic)
            .OnComplete(() => 
            {
                barExp = 0;

                if (++barLevel > curLevel)
                    return;

                _expBar.SetLevel(barLevel);
            }).WaitForCompletion();

            if (barLevel > curLevel)
                yield break;
        }
    }
}
