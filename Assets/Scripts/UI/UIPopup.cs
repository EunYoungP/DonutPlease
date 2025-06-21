using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopup : UIBehaviour
{
    [SerializeField] private GameObject contents;

    public PopupLayerType layerType = 0;

    protected override void OnDestroy()
    {
        if (contents != null)
        {
            contents.transform.localScale = Vector3.one;
            contents.transform.DOKill();
        }

        base.OnDestroy();
    }

    public void PopupOpend()
    {
        PlayShowAnimation();
    }

    public virtual void Show()
    {

    }

    public virtual void Hide()
    {
        Destroy(gameObject);

        PopupClosed();
    }

    public void PopupClosed()
    {
        //world.Flux.Emit(new FxUiPopupClosed(GetType().Name, GetType()));
    }


    private void PlayShowAnimation()
    {
        contents.transform.localScale = Vector3.one * 0.8f;
        contents.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }
}
