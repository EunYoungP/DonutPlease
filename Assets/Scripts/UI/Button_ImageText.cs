using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_ImageText : UIBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;

    private Action _btnCallback;

    protected override void Awake()
    {
        _button.onClick.AddListener(() =>_btnCallback?.Invoke());
    }

    public void SetCallback(Action callback)
    {
        _btnCallback = callback;
    }

    public void SetImage(string imageName)
    {
        var gameMng = GameManager.GetGameManager;
        var path = gameMng.Resource.GetUIResourcePath(imageName);

        _image.sprite = GameManager.GetGameManager.Resource.GetSprite(path);
    }

    public void SetText(string text)
    {
        if (_text != null)
        {
            _text.text = text;
        }
    }
}
