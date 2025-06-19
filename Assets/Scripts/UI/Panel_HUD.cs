using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class Panel_HUD : UIBehaviour
{
    [SerializeField] private ExpBar _expBar;
    [SerializeField] private TextImage _textImageCash;
    [SerializeField] private TextImage _textImageGem;
    [SerializeField] private GameObject _expEffect;

    private Canvas _canvas;
    private RectTransform _rectCanvas;

    protected override void OnEnable()
    {
        _canvas = GameManager.GetGameManager._canvas;
        _rectCanvas = _canvas.GetComponent<RectTransform>();

        GameManager.GetGameManager.Player.Currency.Cash.Subscribe(cash => OnUpdateCash(cash));
        GameManager.GetGameManager.Player.Currency.Gem.Subscribe(gem => OnUpdateGem(gem));

        GameManager.GetGameManager.Player.Growth.Exp.Subscribe(exp => OnUpdateExp(exp));
        //GameManager.GetGameManager.Player.Growth.Level.Subscribe(level => OnUpdateLevel(level));
    }


    private void OnUpdateCash(int cash)
    {
        _textImageCash.SetText(cash);
    }

    private void OnUpdateGem(int gem)
    {
        _textImageGem.SetText(gem);
    }

    private void OnUpdateExp(int exp)
    {
        var maxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value);
        var prevMaxExp = GameManager.GetGameManager.Player.Growth.GetMaxExpByLevel(GameManager.GetGameManager.Player.Growth.Level.Value - 1);

        PlayGetExpEffect();
    }

    private void PlayGetExpEffect()
    {
        var followCamera = GameManager.GetGameManager.Player.Character.Camera.MainCamera;
        var characterPos = GameManager.GetGameManager.Player.Character.gameObject.transform.position;
        var cameraPos = GameManager.GetGameManager.Player.Character.Camera.transform.position;

        // 캐릭터 중간 위치 World -> Local 
        var midPos = followCamera.WorldToScreenPoint(characterPos + new Vector3(0, 1f));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectCanvas, midPos, _canvas.worldCamera, localPoint: out var midLocalPos);
        _expEffect.transform.position = _rectCanvas.TransformPoint(midLocalPos);
        _expEffect.SetActive(true);

        // 별 모양 이동시키고 완료 시점에서 업데이트
        _expEffect.transform.DOMove(_expBar.Level.transform.position, 2.0f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
        {
            _expEffect.SetActive(false);
            _expEffect.transform.position = _expBar.Level.transform.position;
        });
    }
}
