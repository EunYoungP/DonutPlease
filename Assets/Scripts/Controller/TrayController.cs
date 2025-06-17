using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class TrayController : MonoBehaviour
{
    [SerializeField] private Vector2 _shakeRange = new Vector2(0.8f, 0.4f);
    [SerializeField] private float _itemHeight = 0.2f;
    [SerializeField] private float _bandFactor = 0.1f;

    private HashSet<Item> _reserved = new HashSet<Item>();
    private List<Item> _items = new List<Item>();

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

        if (TotalItemCount <= 0)
            return;

        _items[0].transform.position = transform.position;
        _items[0].transform.rotation = transform.rotation;

        for (int i = 1; i < _items.Count; i++)
        {
            float rate = Mathf.Lerp(_shakeRange.x, _shakeRange.y, i / (float)_items.Count);

            _items[i].transform.position = Vector3.Lerp(_items[i].transform.position, _items[i - 1].transform.position + (_items[i - 1].transform.up * _itemHeight), rate);
            _items[i].transform.rotation = Quaternion.Lerp(_items[i].transform.rotation, _items[i - 1].transform.rotation, rate);

            if (dir != Vector3.zero)
                _items[i].transform.rotation *= Quaternion.Euler(0, _bandFactor * rate, i);
        }
    }

    public void PlayAddToTray(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item component is missing on the child object.");
            return;
        }

        // 아이템 동일 타입 체크필요
        if (!CanGetItemType(item.ItemType))
            return;

        _reserved.Add(item);

        Vector3 dest = transform.position + Vector3.up * TotalItemCount * _itemHeight;

        item.transform.DOJump(dest, 5, 1, 0.3f)
            .OnComplete(() =>
            {
                PlayGetItemSFX(item.ItemType);

                _reserved.Remove(item);
                _items.Add(item);

                item.transform.SetParent(transform);
                CheckTrayActivation();
            });
    }

    public void PlayPutDownFromTray(Transform child, PileBase dest)
    {
        Item item = child.GetComponent<Item>();
        if (item == null)
        {
            Debug.LogError("Item component is missing on the child object.");
            return;
        }

        if (_items.Contains(item))
        {
            _items.Remove(item);
            item.transform.DOJump(dest.GetPositionAt(dest.ObjectCount), 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    PlayGetItemSFX(item.ItemType);

                    dest.AddToPile(child.gameObject);
                    child.transform.rotation = Quaternion.identity;

                    CheckTrayActivation();
                });
        }
    }

    public void PlayPutDownFromTray(Transform trash, Transform dest)
    {
        // 아이템 동일 타입 체크필요
        Item item = trash.GetComponent<Item>();
        if (item == null)
        {
            Debug.LogError("Item component is missing on the child object.");
            return;
        }

        // 아이템 동일 타입 체크필요
        if (!CanGetItemType(item.ItemType))
            return;

        if (_items.Contains(item))
        {
            _items.Remove(item);
            trash.DOJump(dest.position, 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    Destroy(trash.gameObject);

                    CheckTrayActivation();
                });
        }
    }

    private void PlayGetItemSFX(EItemType type)
    {
        if (type == EItemType.Donut)
            GameManager.GetGameManager.Audio.PlaySFX(AudioClipNames.pileDonutString);
        else if (type == EItemType.Trash)
            GameManager.GetGameManager.Audio.PlaySFX(AudioClipNames.pileDonutString);
    }

    private void CheckTrayActivation()
    {
        this.gameObject.SetActive(_items.Count > 0);
    }

    private bool CanGetItemType(EItemType itemType)
    {
        if (_items.Count == 0)
            return true;

        if (_items[0].ItemType == itemType)
            return true;

        return false;
    }
}
