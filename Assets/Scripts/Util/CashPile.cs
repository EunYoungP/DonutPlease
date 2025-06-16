using DonutPlease.Game.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CashPile : PileBase
{
    [SerializeField]
    private GameObject _prefab;

    private Coroutine _curCoroutine;
    private CharacterBase _workingCharcater;

    private float _makeInterval = 0.2f;
    private float _gettingInterval = 0.2f;

    public bool IsWorking { get; private set; }

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoopGetCashFromPile(other.GetComponent<CharacterBase>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }

    // 특정 개수만큼 파일에서 캐시 가져오기
    public void LoopGetCashFromPile(CharacterBase workingCharacter)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = workingCharacter;

        IsWorking = true;

        StartCoroutine(CoLoopEnterGetCoroutine());
    }

    // 특정 개수만큼 파일에 캐시 쌓기
    public void MakeCashInPile(CharacterBase character, int count)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = character;

        IsWorking = true;

        StartCoroutine(CoMakeCashByCount(character, count));
    }


    #region Coroutine Loop

    private IEnumerator CoLoopEnterGetCoroutine()
    {
        while (IsWorking && !IsEmpty)
        {
            _curCoroutine = StartCoroutine(CoGetFromPile(_workingCharcater));
            yield return _curCoroutine;
        }

        ResetCoroutine();
    }

    private IEnumerator CoMakeCashByCount(CharacterBase character, int count)
    {
        int getDonutCount = 0;
        while (IsWorking && getDonutCount < count)
        {
            yield return StartCoroutine(CoMakeCashInPile(character));
            getDonutCount++;
        }

        ResetCoroutine();
    }

    #endregion

    #region Coroutine

    private IEnumerator CoGetFromPile(CharacterBase character)
    {
        if (IsEmpty)
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < _gettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject cash = RemoveFromPile();
        FluxSystem.Dispatch(new FxOnGetItem(EItemType.Cash, cash, character));
    }

    private IEnumerator CoMakeCashInPile(CharacterBase character)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _gettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject cash = Instantiate(_prefab, transform.position, Quaternion.identity);
        AddToPile(cash);
    }

    #endregion

    private IEnumerator CoWaitOtherWorking()
    {
        yield return new WaitUntil(() => !IsWorking);
    }

    private void ResetCoroutine()
    {
        IsWorking = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }
}
