using DonutPlease.Game.Character;
using UnityEngine;

public class UIFollowCharacterPlayer : UIFollowCharacterBase
{
    //[SerializeField] private RectTransform _rectRootEmoji;
    [SerializeField] private RectTransform _rectChildEmoji;

    private GameManager _gameMng;
    private Canvas _canvas;
    private RectTransform _rectCanvas;
    private RectTransform _rectTransform;

    private CharacterBase _charcater;
    private float _characterHeight = 1.8f;
    private float _characterWidth = 1f;
    private Vector3 _lastActorPos;
    private Vector3 _lastCamPos;
    private float _lastFov;

    private UIEmoji _usingEmoji;

    private void Awake()
    {
        _canvas = GameManager.GetGameManager._canvas;
        _rectCanvas = _canvas.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void Initialize(CharacterBase character)
    {
        _gameMng = GameManager.GetGameManager;
        if (_gameMng == null)
            return;

        _charcater = character;

        SetChildParent();
    }

    private void LateUpdate()
    {
        UpdatePositionAndScale();
        UpdateChildEmojiPositionAndScale();
    }

    private void UpdatePositionAndScale()
    {
        if (_charcater == null)
            return;

        var followCamera = _gameMng.Player.Character.Camera.MainCamera;
        if (followCamera == null)
            return;

        var characterPos = _charcater.gameObject.transform.position;
        var cameraPos = _gameMng.Player.Character.Camera.transform.position;
        //var actorHeight = _actorHeight - World.Camera.FollowCamera.CurrSettings.UnitUiHeight; 
        //var actorWidth = _actorWidth;
        var cameraFov = followCamera.fieldOfView;

        //Move, 캐릭터/카메라 이동 시
        if (_lastActorPos != characterPos || _lastCamPos != cameraPos || _lastFov != cameraFov)
        {
            // 캐릭터 위치 World -> Local
            var actorScreenPos = followCamera.WorldToScreenPoint(characterPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectCanvas, actorScreenPos,_canvas.worldCamera, out var localPos);
            _rectTransform.anchoredPosition = localPos;

            // 캐릭터 상단 위치 World -> Local 
            var topPos = followCamera.WorldToScreenPoint(characterPos + new Vector3(_characterWidth, _characterHeight));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectCanvas, topPos, _canvas.worldCamera, localPoint: out var topLocalPos);

            //    var anchoredTopPos = topLocalPos - localPos;
            //    _top.anchoredPosition = new Vector2(0, anchoredTopPos.y);
            //    _center.anchoredPosition = new Vector2(0, anchoredTopPos.y / 2);
            //    _rectRootEmoji.sizeDelta = topLocalPos - localPos;

            //    if (IsUsingEmoji())
            //        _usingEmoji.RectTransform.sizeDelta = _rectRootEmoji.sizeDelta;

            //    UpdateScale();

            _lastActorPos = characterPos;
            _lastCamPos = cameraPos;
            _lastFov = cameraFov;
        //    _lastActorHeight = actorHeight;
        //    _lastActorWidth = actorWidth;
        }
    }

    private void UpdateScale()
    {
        // 이모지 활성화 중에만 갱신
        //if (!IsUsingEmoji())
        //{
        //    return;
        //}

        //if (World.CutScene.IsCutScenePlay())
        //{
        //    ResetChildRootScale();
        //    return;
        //}

        //Transform cameraTr = World.Camera.FollowCamera.Camera.transform;
        //float CamTargetDistance = cameraTr.position.z - World.Camera.FollowCamera.Target.z;
        //float CamActorDistance = cameraTr.position.z - _actor.Transform.Position.z;

        //float ratio = CamTargetDistance / CamActorDistance;
        //float rawScale = World.Camera.FollowCamera.CurrSettings.UnitUiScale;

        //if (CamTargetDistance != 0)
        //    rawScale *= ratio;

        //float adjustedScale = Mathf.Max(0f, Mathf.Min(2f, rawScale));

        //SetChildRootScale(new Vector3(adjustedScale, adjustedScale, adjustedScale));
    }

    private void UpdateChildEmojiPositionAndScale()
    {
        //if (IsUsingEmoji() && (_rectChildEmoji.position != _rectRootEmoji.position))
        //{
        //    _rectChildEmoji.position = _rectRootEmoji.position;
        //}
    }

    private void UpdateEmojiVisible()
    {
        //var interactableAndEmojiVisible = _canvasGroup.interactable && _isEmojiVisible;
        //if (_isInteractableAndEmojiVisible != interactableAndEmojiVisible)
        //{
        //    _isInteractableAndEmojiVisible = interactableAndEmojiVisible;

        //    if (_rectChildEmoji.IsActiveSelf() != interactableAndEmojiVisible)
        //        _rectChildEmoji.SetActive(interactableAndEmojiVisible);
        //}
    }

    public override void ShowEmoji(string emojiName)
    {
        //if (_rectChildEmoji.IsActiveSelf() == false)
        //    return;

        if (IsUsingEmoji())
            GameManager.GetGameManager.Pool.UIEmojiPool.Release(_usingEmoji);

        //_balloon.SetActive(false);

        //말풍선 위치 정상 출력을 위함
        //_rectRootEmoji.SetActive(false);
        //_rectRootEmoji.SetActive(true);

        UIEmoji uiEmoji = GameManager.GetGameManager.Pool.UIEmojiPool.Get(emojiName);
        _usingEmoji = uiEmoji;

        if (uiEmoji != null && uiEmoji.RectTransform != null)
        {
            //이모지 루트에 스케일 대응값 전달
            //uiEmoji.SetScale(_center.localScale);
            //uiEmoji.RectTransform.sizeDelta = _rectRootEmoji.sizeDelta;
            uiEmoji.RectTransform.SetParent(_rectChildEmoji, false);

            _rectChildEmoji.SetAsLastSibling();
            uiEmoji.RectTransform.SetAsLastSibling();
            //uiEmoji.Show(emojiId, () => _usingEmoji = null);
        }
    }

    public override void HideEmoji()
    {
        //if (IsUsingEmoji())
        //{
        //    _usingEmoji.ReleasePool();
        //    _usingEmoji = null;
        //}

        //_rectRootEmoji.SetActive(false);
    }

    private bool IsUsingEmoji()
    {
        if (_usingEmoji == null)
            return false;

        return true;
    }


    private UIFollowCharacterPlayer SetChildParent()
    {
        //RectTransform rect;

        //rect = World.Scene.SceneGame.GetUIFollow.rectMyHP;

        //_rectChildHp.SetParent(rect);
        //_rectChildHp.SetAsFirstSibling();

        //rect = World.Scene.SceneGame.GetUIFollow.rectMyName;

        //_rectChildName.SetParent(rect);
        //_rectChildName.SetAsFirstSibling();

        //_textChildClanName.transform.SetParent(rect);
        //_textChildClanName.transform.SetAsFirstSibling();

        //rect = World.Scene.SceneGame.GetUIFollow.rectMyBalloon;

        //_rectChildBalloon.SetParent(rect);
        //_rectChildBalloon.SetAsFirstSibling();

        //_rectChildHeal.SetParent(World.Scene.SceneGame.GetUIFollow.rectDamage);
        //_rectChildDamage.SetParent(World.Scene.SceneGame.GetUIFollow.rectDamage);

        _rectChildEmoji.SetParent(_gameMng.GetUIFollowCharacterRoot.EmojiRoot.transform);
        //_rectChildActorHeadNotification.SetParent(World.Scene.SceneGame.GetUIFollow.rectContentNoti);

        //var localScale = GetResourceFixedSize();

        //_rectChildActorHeadNotification.localScale = localScale;
        //_rectChildDamage.localScale = localScale;
        //_rectChildHeal.localScale = localScale;
        //_rectChildBalloon.localScale = localScale;
        //_rectChildName.localScale = localScale;
        //_rectChildClanName.localScale = localScale;
        //_rectChildHp.localScale = localScale;

        return this;
    }
}
