using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DonutPlease.Game.Character;
using DonutPlease.UI;
using DG.Tweening;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[System.Serializable]
public struct InteractionProp
{
    public int Id;
    public int StoreId;
    public Vector3 Pos;
    public Vector3 Rot;
    public InteractionType Type;

    // Create Interaction UI일 경우
    public int[] NextIds;
    public int NeedCash;
    public int RewardExp;
}

public class LocalMapSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

    [SerializeField]
    private Transform _uIRoot;

    [SerializeField]
    private GameObject _map;

    [SerializeField]
    private GameObject _officeHRProps;

    [SerializeField]
    private GameObject _officeUpgradeProps;

    [SerializeField]
    public List<InteractionProp> _interactionPropDatas;

    public GameObject Map => _map;
    public GameObject OfficeHRProps => _officeHRProps;
    public GameObject OfficeUpgradeProps => _officeUpgradeProps;

    public void Initialize()
    {
        FluxSystem.ColliderTriggerActionStream.Subscribe(data =>
        {
            if (data is FxOnTriggerEnter fxTriggerEnter)
            {
                if (fxTriggerEnter.characterBase is CharacterPlayer player)
                {
                    if (fxTriggerEnter.colliderType == EColliderIdentifier.InHR || fxTriggerEnter.colliderType == EColliderIdentifier.InUpgrade)
                        OnTriggerEnterOffice(fxTriggerEnter.colliderType);
                }
            }

            if (data is FxOnTriggerExit fxTriggerExit)
            {
                if (fxTriggerExit.characterBase is CharacterPlayer player)
                {
                    if (fxTriggerExit.colliderType == EColliderIdentifier.InHR || fxTriggerExit.colliderType == EColliderIdentifier.InUpgrade)
                        OnTriggerExitOffice(fxTriggerExit.colliderType);
                }
            }
        }).AddTo(this);
    }


    public InteractionProp GetPropData(int id)
    {
        foreach (var propData in _interactionPropDatas)
        {
            if (propData.Id == id)
            {
                return propData;
            }
        }
        return default;
    }

    // 프랍 생성은 무조건 여기에서 실행.
    public void CreateProp(int id)
    {
        InteractionProp prop = GetPropData(id);

        GameObject propRoot = new GameObject("propPos");
        propRoot.transform.SetParent(_root.transform);

        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(prop.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);

        propObj.transform.localScale = Vector3.one * 0.7f;
        propObj.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);

        propObj.transform.SetParent(propRoot.transform);

        propRoot.transform.localPosition = prop.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(prop.Rot);

        SetStore(propObj, prop);
    }

    private void SetStore(GameObject propObj, InteractionProp prop)
    {
        Store store = GameManager.GetGameManager.Store.GetStore(prop.StoreId);

        switch (prop.Type)
        {
            case InteractionType.OpenFrontDoor:
                break;
            case InteractionType.CreateTable:
                Table table = propObj.GetComponent<Table>();
                store.AddTable(table);
                break;
            case InteractionType.CreateMachine:
                Machine machine = propObj.GetComponent<Machine>();
                store.AddMachine(machine);
                break;
            case InteractionType.CreateCounter:
                Counter counter = propObj.GetComponent<Counter>();
                store.AddMainCouter(counter);
                break;
            case InteractionType.OpenDriveThru:
                break;
            case InteractionType.OpenHR:
                break;
        }
    }

    public void CreateInteractionUI(int id)
    {
        GameObject propRoot = new GameObject($"UIInteraction_{id}");
        propRoot.transform.SetParent(_uIRoot.transform);

        InteractionProp propData = GetPropData(id);
        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(propData.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        UIInteraction uIInteraction = propObj.GetComponent<UIInteraction>();
        uIInteraction.SetId(id);

        // Set Callback
        foreach (int nextId in propData.NextIds)
        {
            uIInteraction.AddCallback(() => GameManager.GetGameManager.Intercation.ActiveInteraction(nextId));
        }

        propRoot.transform.localPosition = propData.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(propData.Rot);
    }

    #region Office

    private void OnTriggerEnterOffice(EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.InHR)
        {
            //  HR 팝업 출력
            GameManager.GetGameManager.Popup.Show<Popup_HR>();
        }
        else if (identifier == EColliderIdentifier.InUpgrade)
        {
            // Upgrad 팝업 출력
            GameManager.GetGameManager.Popup.Show<Popup_PlayerUpgrade>();
        }
    }

    private void OnTriggerExitOffice(EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.InHR)
        {
            //  HR 팝업 출력
            GameManager.GetGameManager.Popup.Hide<Popup_HR>();
        }
        else if (identifier == EColliderIdentifier.InUpgrade)
        {
            // Upgrad 팝업 출력
            GameManager.GetGameManager.Popup.Hide<Popup_PlayerUpgrade>();
        }
    }

    #endregion
}
