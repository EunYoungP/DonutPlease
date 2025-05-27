using DonutPlease.Game.Character;
using System.Collections;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Machine : PropBase
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private GameObject _prefab;

    private float _makeInterval = 0.2f;
    private float _getInterval = 0.2f;

    private bool _isGettingDonut;

    private CharacterBase _enterCharcater;
    private Coroutine _curCoroutine;

    public DonutPile DonutPile => _donutPile;
    public int DonutCount => _donutPile.ObjectCount;

    private void OnEnable()
    {
        _curCoroutine = StartCoroutine(_donutPile.CoLoopMakeDonut());
    }

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DonutCount > 0)
            {
                ResetCoroutine();
                _isGettingDonut = true;
                _enterCharcater = other.GetComponent<CharacterBase>();

                StartCoroutine(CoLoopGetDonutFromPile());
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

            _curCoroutine = StartCoroutine(_donutPile.CoLoopMakeDonut());
        }
    }

    private IEnumerator CoLoopGetDonutFromPile()
    {
        while (_isGettingDonut)
        {
            _curCoroutine = StartCoroutine(CoGetDonutFromPile());
            yield return _curCoroutine;
        }
        yield return null;
    }

    private IEnumerator CoGetDonutFromPile()
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

        yield return null;
    }

    private void ResetCoroutine()
    {
        _isGettingDonut = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }
}
