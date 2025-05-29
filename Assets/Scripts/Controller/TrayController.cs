using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

enum ETrayItem
{
    Donut,
    Trash,
}

public class TrayController : MonoBehaviour
{
    [SerializeField] private Vector2 _shakeRange = new Vector2(0.8f, 0.4f);
    [SerializeField] private float _itemHeight = 0.2f;
    [SerializeField] private float _bandFactor = 0.1f;

    private HashSet<Transform> _reserved = new HashSet<Transform>();
    private List<Transform> _items = new List<Transform>();

    private CharacterBase _character;
    private float characterV;
    private float characterH;

    private bl_Joystick _joystick;
    private float _cameraRot;

    public int TotalItemCount => _items.Count;

    private void Awake()
    {
        _joystick = GameManager.GetGameManager.JoyStick;
        _cameraRot = FindAnyObjectByType<PlayerCamera>().RoatateY;
    }

    private void Update()
    {
        TiltItem();
    }

    public TrayController SetCharacter(CharacterBase character)
    {
        if (_character == null)
            _character = character;
        return this;
    }

    private void SetDirection()
    {
        if (_character is CharacterPlayer player)
        {
            characterV = _joystick.Vertical;
            characterH = _joystick.Horizontal;
        }
        else if (_character is CharacterCustomer customer)
        {
            characterV = customer.Controller.GetVelocity().z;
            characterH = customer.Controller.GetVelocity().x;
        }
        else if (_character is CharacterWorker worker)
        {
            characterV = worker.Controller.GetVelocity().z;
            characterH = worker.Controller.GetVelocity().x;
        }
    }

    private void TiltItem()
    {
        SetDirection();

        Quaternion cameraRot = Quaternion.Euler(0, _cameraRot, 0);
        Vector3 dir = cameraRot * new Vector3(characterH, 0, characterV);

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
        // 아이템 동일 타입 체크필요

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

    public void PlayPutDownFromTray(Transform trash, Transform dest)
    {
        // 아이템 동일 타입 체크필요

        if (_items.Contains(trash))
        {
            _items.Remove(trash);
            trash.DOJump(dest.position, 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    Destroy(trash.gameObject);
                });
        }

        CheckTrayActivation();
    }

    private void CheckTrayActivation()
    {
        this.gameObject.SetActive(_items.Count > 0);
    }
}
