using System.Collections;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Machine : MonoBehaviour
{
    private int CurBurger => _donutPile.ObjectCount;

    [SerializeField]
    private PileBase _donutPile;

    [SerializeField]
    private GameObject _prefab;

    private float _makeInterval = 0.2f;
    private float _getInterval = 0.2f;

    private bool _isGettingDonut;

    private void OnEnable()
    {
        StartCoroutine(CoMakeDonut());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CurBurger > 0)
            {
                _isGettingDonut = true;

                StartCoroutine(CoGetDonut());
            }
            else
            {
                Debug.Log("도넛이 없습니다.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CoMakeDonut());
        }
    }

    private IEnumerator CoGetDonut()
    {
        if (_donutPile.IsEmpty)
        {
            _isGettingDonut = false;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _getInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject burger = _donutPile.RemoveFromPile();
        FluxSystem.Dispatch(new OnPlyaerGetDonut(burger));
        
        //Destroy(burger);


        Debug.Log("도넛 삭제");

        StartCoroutine(CoGetDonut());
    }

    private IEnumerator CoMakeDonut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _makeInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (_isGettingDonut)
            yield break;

        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        _donutPile.AddToPile(go);

        Debug.Log("도넛 만들기");

        if (!_donutPile.IsFull)
            StartCoroutine(CoMakeDonut());
    }
}
