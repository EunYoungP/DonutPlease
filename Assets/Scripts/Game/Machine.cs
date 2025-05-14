using DonutPlease.Game.Character;
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

    private CharacterBase _enterCharcater;
    private Coroutine _curCoroutine;


    private void OnEnable()
    {
        _curCoroutine = StartCoroutine(CoMakeDonut());
    }

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CurBurger > 0)
            {
                _isGettingDonut = true;
                _enterCharcater = other.GetComponent<CharacterBase>();

                StartCoroutine(CoLoopGetFromPileCoroutine());
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
            ResetCoroutine();

            _curCoroutine = StartCoroutine(CoMakeDonut());
        }
    }

    private IEnumerator CoLoopGetFromPileCoroutine()
    {
        while (_isGettingDonut)
        {
            _curCoroutine = StartCoroutine(CoGetFromPile());
            yield return _curCoroutine;
        }
        yield return null;
    }

    private IEnumerator CoGetFromPile()
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

        GameObject donut = _donutPile.RemoveFromPile();
        FluxSystem.Dispatch(new OnGetDonut(donut, _enterCharcater));
    }

    private IEnumerator CoMakeDonut()
    {
        if (_isGettingDonut)
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < _makeInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        _donutPile.AddToPile(go);

        Debug.Log("도넛 만들기");

        if (!_donutPile.IsFull)
            StartCoroutine(CoMakeDonut());
    }

    private void ResetCoroutine()
    {
        _isGettingDonut = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }
}
