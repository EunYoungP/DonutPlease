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

    // ����Ʈ �ý��� ������ �����ؾ���.
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

    // ���� Id�� �´� ����� ��ġ�� ������Ʈ ����
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

