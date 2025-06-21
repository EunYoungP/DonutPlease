using System.Collections.Generic;
using UnityEngine;


public enum PopupLayerType : int
{
    Alert = 0,
    Common
}

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

        var parent = GetPopupParent(popup.layerType);
        popup.transform.SetParent(parent, false);

        return popup;
    }

    private Transform GetPopupParent(PopupLayerType layerType)
    {
        if (layerType == PopupLayerType.Alert)
            return GameManager.GetGameManager._alertPopupsRoot.transform;

        return GameManager.GetGameManager._popupsRoot.transform;
    }

    public void Hide<T>() where T : UIPopup
    {
        Hide(typeof(T).Name);
    }

    private void Hide(string popupName)
    {
        UIPopup popup = _popups.Find(p => p.GetType().Name == popupName);
        if (popup != null)
        {
            popup.Hide();
            _popups.Remove(popup);
        }
    }
}
