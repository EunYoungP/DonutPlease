using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct InteractionProp
{
    public int Id;
    public Vector3 Pos;
    public Vector3 Rot;
    public GameObject Prefab;
}

public class LocalMapSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

    [SerializeField]
    private Transform _uIRoot;

    // 퀘스트 시스템 등으로 변경해야함.
    [SerializeField]
    public List<InteractionProp> _props;

    public void Initialize()
    {
    }

    private InteractionProp GetProp(int id)
    {
        foreach (var prop in _props)
        {
            if (prop.Id == id)
            {
                return prop;
            }
        }

        return default;
    }

    // 생성 Id에 맞는 저장된 위치에 오브젝트 생성
    public void CreateProp(int id)
    {
        InteractionProp prop = GetProp(id);

        GameObject propRoot = new GameObject("propPos");
        propRoot.transform.SetParent(_root.transform);

        GameObject propObj = Instantiate(prop.Prefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        propRoot.transform.localPosition = prop.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(prop.Rot);
    }

    public void CreateInteractionUI(int id)
    {
        InteractionProp prop = GetProp(id);

        GameObject propRoot = new GameObject($"UIInteraction_{id}");
        propRoot.transform.SetParent(_uIRoot.transform);

        GameObject propObj = Instantiate(prop.Prefab, Vector3.zero, Quaternion.identity);
        propObj.transform.SetParent(propRoot.transform);

        UIInteraction uIInteraction = propObj.GetComponent<UIInteraction>();
        //uIInteraction .AddData(id + 1)

        propRoot.transform.localPosition = prop.Pos;
        propRoot.transform.localRotation = Quaternion.Euler(prop.Rot);
    }
}

