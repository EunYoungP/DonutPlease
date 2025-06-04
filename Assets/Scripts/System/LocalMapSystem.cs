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
    public List<InteractionProp> _interactionPropDatas;

    public GameObject Map => _map;

    public void Initialize()
    {
    }

    private InteractionProp GetPropData(int id)
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
    public void CreateProp(int id, InteractionType type)
    {
        var propObj = CreateProp(id);

        InteractionProp prop = GetPropData(id);

        Store store = GameManager.GetGameManager.Store.GetStore(prop.StoreId);

        switch (type)
        {
            case InteractionType.Open:
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
            case InteractionType.CreateDriveThru:
                break;
            case InteractionType.HireStaff:
                break;
        }
        
    }

    private GameObject CreateProp(int id)
    {
        InteractionProp prop = GetPropData(id);

        GameObject propRoot = new GameObject("propPos");
        propRoot.transform.SetParent(_root.transform);

        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(prop.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        propRoot.transform.localPosition = prop.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(prop.Rot);

        return propObj;
    }

    public void CreateInteractionUI(int id, int nextInterationId)
    {
        InteractionProp propData = GetPropData(id);
        InteractionProp nextPropData = GetPropData(nextInterationId);

        GameObject propRoot = new GameObject($"UIInteraction_{id}");
        propRoot.transform.SetParent(_uIRoot.transform);

        GameObject propPrefab = GameManager.GetGameManager.Resource.GetPropByType(propData.Type);
        GameObject propObj = Instantiate(propPrefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        UIInteraction uIInteraction = propObj.GetComponent<UIInteraction>();

        uIInteraction.AddData(nextInterationId, nextPropData.Type);

        propRoot.transform.localPosition = propData.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(propData.Rot);
    }
}

