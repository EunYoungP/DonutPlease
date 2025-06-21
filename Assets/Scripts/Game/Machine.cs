using DonutPlease.Game.Character;
using UnityEngine;

public class Machine : PropBase
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private Transform _donutPileFrontPos;

    private float _makeInterval = 0.2f;
    private float _getInterval = 0.2f;

    private bool _isGettingDonut;

    private CharacterBase _enterCharcater;
    private Coroutine _curCoroutine;

    public DonutPile DonutPile => _donutPile;
    public int DonutCount => _donutPile.ObjectCount;
    public Transform DonutPileFrontPosition => _donutPileFrontPos;

    private void OnEnable()
    {
        StartCoroutine(_donutPile.CoLoopMakeDonut());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DonutCount > 0)
            {
                ResetCoroutine();

                var enterCharcater = other.GetComponent<CharacterBase>();
                _donutPile.LoopGetDonutFromPile(enterCharcater);
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
            DonutPile.OnTriggerExitGetDonut();
        }
    }

    private void ResetCoroutine()
    {
        _isGettingDonut = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }
}
