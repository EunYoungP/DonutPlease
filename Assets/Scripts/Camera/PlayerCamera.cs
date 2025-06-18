using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;
using Sequence = DG.Tweening.Sequence;

public class PlayerCamera : MonoBehaviour
{
    private CharacterPlayer _player;
    private Camera _mainCamera;

    [SerializeField]
    private Vector3 _offset;

    public float RoatateX { get; private set; } =  45f;
    public float RoatateY { get; private set; } = -45f;

    private Vector3 playerBeforePos;
    public bool IsZooming { get; private set; } = false;

    private void LateUpdate()
    {
        if (IsZooming)
            return;

        FollowPlayer();
    }

    public void Initialize(CharacterPlayer player)
    {
        _player = player;
        _mainCamera = Camera.main;

        if (_player != null)
            playerBeforePos = _player.transform.position;

        Quaternion rotation = Quaternion.Euler(RoatateX, RoatateY, 0f);
        _mainCamera.transform.rotation = rotation;

        Vector3 offset = rotation * _offset;
        _mainCamera.transform.position = playerBeforePos + offset;
    }

    private void FollowPlayer()
    {
        if (_player == null)
            return;

        Vector3 playerCurPos = _player.transform.position;

        Vector3 tmp = transform.position;
        tmp += (playerCurPos - playerBeforePos);
        transform.position = tmp;

        playerBeforePos = playerCurPos;
    }

    public IEnumerator MoveToTarget(Transform target, float moveDuration = 1.5f)
    {
        float fixedY = _mainCamera.transform.position.y;

        Vector3 offset = transform.rotation * _offset;
        Vector3 targetPos = target.position + offset;
        targetPos.y = fixedY;

        float dist = Vector3.Distance(_mainCamera.transform.position, targetPos);
        Debug.Log($"[MoveToTarget] target: {target.name}, 거리: {dist}, duration: {moveDuration}");

        if (dist < 0.01f)
        {
            Debug.Log("거의 같은 위치. 트윈 생략");
            yield break;
        }

        Debug.Log("두트윈 Start");
        yield return _mainCamera.transform.DOMove(targetPos, moveDuration).WaitForCompletion();
        Debug.Log("두트윈 End");
    }

    public void ZoomToTarget(Transform target, float moveDuration = 1.5f, float waitDuration = 2f)
    {
        IsZooming = true;

        var originFOV = _mainCamera.fieldOfView;
        var cameraOriginPos = _mainCamera.transform.position;
        var zoomFOV = 30F;
        var zoomOutDuration = 1f;

        Sequence seq = DOTween.Sequence();

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, _mainCamera.transform.position.z);

        seq.Append(_mainCamera.transform.DOMove(targetPos, moveDuration));
        seq.Join(_mainCamera.DOFieldOfView(zoomFOV, moveDuration));
        seq.AppendInterval(waitDuration);
        seq.Append(_mainCamera.transform.DOMove(cameraOriginPos, moveDuration));
        seq.Join(_mainCamera.DOFieldOfView(originFOV, zoomOutDuration));

        seq.OnComplete(() =>
        {
            IsZooming = false;

            seq.Kill();
        });
    }
}
