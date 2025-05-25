using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrayController : MonoBehaviour
{
    [SerializeField] private Vector2 _shakeRange = new Vector2(0.8f, 0.4f);
    [SerializeField] private float _itemHeight = 0.2f;
    [SerializeField] private float _bandFactor = 0.1f;

    private HashSet<Transform> _reserved = new HashSet<Transform>();
    private List<Transform> _items = new List<Transform>();

    private bl_Joystick _joystick;
    private float _cameraRot;

    private int TotalItemCount => _items.Count;

    private void Awake()
    {
        _joystick = GameManager.GetGameManager.JoyStick;
        _cameraRot = FindAnyObjectByType<PlayerCamera>().RoatateY;
    }

    private void Update()
    {
        TiltItem();
    }

    private void TiltItem()
    {
        float v = _joystick.Vertical;
        float h = _joystick.Horizontal;

        Quaternion cameraRot = Quaternion.Euler(0, _cameraRot, 0);
        Vector3 dir = cameraRot * new Vector3(h, 0, v);

        _items[0].position = transform.position;
        _items[0].rotation = transform.rotation;

        for (int i = 1; i < _items.Count; i++)
        {
            float rate = Mathf.Lerp(_shakeRange.x, _shakeRange.y, i / (float)_items.Count);

            _items[i].position = Vector3.Lerp(_items[i].position, _items[i - 1].position + (_items[i - 1].up * _itemHeight), rate);
            _items[i].rotation = Quaternion.Lerp(_items[i].rotation, _items[i - 1].rotation, rate);

            if (dir != Vector3.zero)
                _items[i].rotation *= Quaternion.Euler(0, _bandFactor * rate, i);
        }
    }

    public void PlayAddToTray(Transform child)
    {
        _reserved.Add(child);

        Vector3 dest = transform.position + Vector3.up * TotalItemCount * _itemHeight;

        child.DOJump(dest, 5, 1, 0.3f)
            .OnComplete(() =>
            {
                _reserved.Remove(child);
                _items.Add(child);

                child.SetParent(transform);
                CheckTrayActivation();
            });
    }

    public void PlayPutDownFromTray(Transform child, PileBase dest)
    {
        if (_items.Contains(child))
        {
            _items.Remove(child);
            child.DOJump(dest.GetPositionAt(dest.ObjectCount), 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    dest.AddToPile(child.gameObject);
                    child.transform.rotation = Quaternion.identity;
                });
        }

        CheckTrayActivation();
    }

    private void CheckTrayActivation()
    {
        this.gameObject.SetActive(_items.Count > 0);
    }
}
