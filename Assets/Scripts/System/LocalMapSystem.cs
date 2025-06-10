using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct InteractionProp
{
    public int Id;
    public int StoreId;
    public Vector3 Pos;
    public Vector3 Rot;
    public InteractionType Type;
    public int[] NextIds;
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
        InteractionProp propData = GetPropData(id);

        // NextIds로 연결된 InteractionProp을 가져옴
        foreach(int nextId in propData.NextIds)
        {
            // 생성될 InteractionPorp이 있다면 콜백으로 넘겨야함.
            GameManager.GetGameManager.Intercation.ActiveInteraction(nextId);
        }

        GameObject propRoot = new GameObject($"UIInteraction_{id}");
        propRoot.transform.SetParent(_uIRoot.transform);

        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(propData.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        UIInteraction uIInteraction = propObj.GetComponent<UIInteraction>();

        // 기존에는 생성되는 UIInteraction에 어떤 타입을 부여할지 여기서 정의,
        // 이제는 
        //uIInteraction.AddData(nextInterationId, nextPropData.Type);

        propRoot.transform.localPosition = propData.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(propData.Rot);
    }
}

