using DG.Tweening;
using DonutPlease.Game.Character;
using DonutPlease.UI;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

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
    public Dictionary<int, GameObject> PropsInStore { get; private set; } = new();  // 생성된 프롭 (프롭 + UIIntercation)

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


    public InteractionProp GetInteractionPropData(int id)
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
        // 프랍 데이터 가져오기
        InteractionProp prop = GetInteractionPropData(id);

        // 프랍 붙일 루트 생성
        GameObject propRoot = new GameObject("propPos");
        propRoot.transform.SetParent(_root.transform);

        // 프랍 프리팹, 객체 생성
        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(prop.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        PropsInStore.Add(id, propObj);

        propRoot.transform.localPosition = prop.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(prop.Rot);

        // 프랍 생성 스케일 효과
        Vector3 originScale = propObj.transform.localScale;
        propObj.transform.localScale = originScale * 0.6f;
        propObj.transform.DOScale(originScale, 0.25f).SetEase(Ease.OutBack);

        SetStore(propObj, prop);
    }

    public void CreateInteractionUI(int id)
    {
        // 프랍 붙일 루트 생성
        GameObject propRoot = new GameObject($"UIInteraction_{id}");
        propRoot.transform.SetParent(_uIRoot.transform);

        // 프랍 데이터 가져오기
        InteractionProp propData = GetInteractionPropData(id);

        // 프랍 프리팹, 객체 생성
        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(propData.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        PropsInStore.Add(id, propObj);

        // UIInteaction 가져오기
        UIInteraction uIInteraction = propObj.GetComponent<UIInteraction>();
        uIInteraction.SetId(id);

        // 콜백 등록
        foreach (int nextId in propData.NextIds)
            uIInteraction.AddCallback(() => GameManager.GetGameManager.Intercation.ActiveInteraction(nextId));
        uIInteraction.AddCallback(() => Destroy(uIInteraction.gameObject));

        propRoot.transform.localPosition = propData.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(propData.Rot);
    }

    public void SetActiveProp(int id)
    {
        InteractionProp propData = GetInteractionPropData(id);

        if (propData.Type == InteractionType.OpenHR)
        {
            PropsInStore.Add(id, OfficeHRProps);

            Vector3 originScale = OfficeHRProps.transform.localScale;
            OfficeHRProps.transform.localScale = originScale * 0.6f;
            OfficeHRProps.SetActive(true);
            OfficeHRProps.transform.DOScale(originScale, 0.25f).SetEase(Ease.OutBack);
        }
        else if (propData.Type == InteractionType.OpenUpgrade)
        {
            PropsInStore.Add(id, OfficeUpgradeProps);

            Vector3 originScale = OfficeUpgradeProps.transform.localScale;
            OfficeUpgradeProps.transform.localScale = originScale * 0.6f;
            OfficeUpgradeProps.SetActive(true);
            OfficeUpgradeProps.transform.DOScale(originScale, 0.25f).SetEase(Ease.OutBack);
        }
        else if (propData.Type == InteractionType.OpenDriveThru)
        {
            var driveThruObj = Resources.FindObjectsOfTypeAll<DriveThru>().First().gameObject;
            PropsInStore.Add(id, driveThruObj);

            Vector3 originScale = driveThruObj.transform.localScale;
            driveThruObj.transform.localScale = originScale * 0.6f;
            driveThruObj.SetActive(true);
            driveThruObj.transform.DOScale(originScale, 0.25f).SetEase(Ease.OutBack);

            SetStore(driveThruObj, propData);
        }
    }

    public GameObject GetProp(int id)
    {
        return PropsInStore.TryGetValue(id, out GameObject prop) ? prop : null;
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
                DriveThru driveThru = propObj.GetComponent<DriveThru>();
                store.AddDriveThru(driveThru);
                break;
        }
    }

    #region Office

    private void OnTriggerEnterOffice(EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.InHR)
        {
            //  HR 팝업 출력
            if (_officeHRProps.activeSelf == false)
                return;

            GameManager.GetGameManager.Popup.Show<Popup_HR>();
        }
        else if (identifier == EColliderIdentifier.InUpgrade)
        {
            if (_officeUpgradeProps.activeSelf == false)
                return;

            // Upgrad 팝업 출력
            GameManager.GetGameManager.Popup.Show<Popup_PlayerUpgrade>();
        }
    }

    private void OnTriggerExitOffice(EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.InHR)
        {
            if (_officeHRProps.activeSelf == false)
                return;

            GameManager.GetGameManager.Popup.Hide<Popup_HR>();
        }
        else if (identifier == EColliderIdentifier.InUpgrade)
        {
            if (_officeUpgradeProps.activeSelf == false)
                return;

            // Upgrad 팝업 출력
            GameManager.GetGameManager.Popup.Hide<Popup_PlayerUpgrade>();
        }
    }

    #endregion
}
