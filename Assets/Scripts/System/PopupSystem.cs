using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

public class PopupSystem
{
    private List<UIPopup> _popups = new();

    public T Show<T>() where T : UIPopup
    {
        return Show(typeof(T).Name) as T;
    }

    private UIPopup Show(string popupName)
    {
        GameObject go = null;

        var loadedGameObject = Resources.Load($"Prefabs/UI/{popupName}", typeof(GameObject)) as GameObject;
        if (loadedGameObject != null)
        {
            go = Object.Instantiate(loadedGameObject);
        }

        return ShowPopup(go);
    }

    private UIPopup ShowPopup(GameObject obj)
    {
        var popup = obj.GetComponent<UIPopup>();

        return ShowPopup(popup);
    }

    private UIPopup ShowPopup(UIPopup popup)
    {
        _popups.Insert(0, popup);

        popup.gameObject.SetActive(true);
        popup.PopupOpend();

        var parent = GameManager.GetGameManager._popupsRoot.transform;
        popup.transform.SetParent(parent, false);

        return popup;
    }
}
