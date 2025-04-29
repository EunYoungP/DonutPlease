using System.Collections;
using UnityEngine;

public class CashPile : PileBase
{
    [SerializeField]
    private GameObject _prefab;

    private float _makeInterval = 0.2f;
    private float _getInterval = 0.2f;

    private bool _isAddingCash;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnTriggerEnter");

            if (ObjectCount > 0)
            {
                _isAddingCash = true;

                StartCoroutine(CoRemoveCash());
            }
            else
            {
                Debug.Log("ĳ�ð� �����ϴ�.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnTriggerExit");

            StartCoroutine(CoMakeCash());
        }
    }

    private IEnumerator CoRemoveCash()
    {
        if (IsEmpty)
        {
            _isAddingCash = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _getInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject burger = RemoveFromPile();
        Destroy(burger);

        Debug.Log("ĳ�� ����");

        StartCoroutine(CoRemoveCash());
    }

    private IEnumerator CoMakeCash()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _makeInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (_isAddingCash)
            yield break;

        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        AddToPile(go);

        Debug.Log("ĳ�� �����");

        if (!IsFull)
            StartCoroutine(CoMakeCash());
    }

}
