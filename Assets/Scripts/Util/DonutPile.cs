using DonutPlease.Game.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DonutPile : PileBase
{
    [SerializeField] private GameObject _prefab;

    private Coroutine _curCoroutine;

    private CharacterBase _workingCharcater;

    private float _makingInterval = 0.2f;
    private float _gettingInterval = 0.2f;

    public bool IsWorking { get; private set; } = false;

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    // �÷��̾� ����
    public void OnTriggerEnterGetDonut(CharacterBase character)
    {
        LoopGetDonutFromPile(character);
    }

    // �÷��̾� ����
    public void OnTriggerEnterTakeDonut(CharacterBase character)
    {
        LoopMoveDonutToPile(character);
    }

    // �÷��̾� ����
    public void OnTriggerExitGetDonut()
    {
        _workingCharcater = null;

        ResetCoroutine();
    }

    // ���� ������ ��� ���� ĳ���ͷ� �̵��ɶ����� �ݺ�
    public void LoopGetDonutFromPile(CharacterBase character)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = character;

        IsWorking = true;

        StartCoroutine(CoLoopEnterGetCoroutine());
    }

    // ĳ������ ��� ���� ���Ϸ� �̵��ɶ����� �ݺ�
    public void LoopMoveDonutToPile(CharacterBase character)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = character;

        IsWorking = true;

        StartCoroutine(CoLoopEnterTakeCoroutine());
    }

    // Ư�� ������ŭ ���Ͽ��� ���� ������
    public void GetDonutFromPileByCount(CharacterBase character, int count)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = character;

        IsWorking = true;

        StartCoroutine(CoGetFromPileByCount(character, count));
    }

    // Ư�� ������ŭ ���Ͽ� ���� �ױ�
    public void MoveDonutToPile(CharacterBase character, int count)
    {
        StartCoroutine(CoWaitOtherWorking());

        _workingCharcater = character;

        IsWorking = true;

        StartCoroutine(CoMoveToPileByCount(character, count));
    }

    #region Coroutine Loop

    private IEnumerator CoLoopEnterGetCoroutine()
    {
        while (IsWorking && CheckPileDonutExist())
        {
            _curCoroutine = StartCoroutine(CoGetFromPile(_workingCharcater));
            yield return _curCoroutine;
        }

        ResetCoroutine(); 
    }

    private IEnumerator CoLoopEnterTakeCoroutine()
    {
        while (IsWorking && CheckCharacterDonutExist(_workingCharcater))
        {
            _curCoroutine = StartCoroutine(CoMoveToPile(_workingCharcater));
            yield return _curCoroutine;
        }

        ResetCoroutine();
    }

    public IEnumerator CoGetFromPileByCount(CharacterBase character, int count)
    {
        int getDonutCount = 0;
        while (IsWorking && getDonutCount < count)
        {
            yield return StartCoroutine(CoGetFromPile(character));
            getDonutCount++;
        }

        ResetCoroutine();
    }

    private IEnumerator CoMoveToPileByCount(CharacterBase character, int count)
    {
        int getDonutCount = 0;
        while (IsWorking && getDonutCount < count)
        {
            yield return StartCoroutine(CoMoveToPile(character));
            getDonutCount++;
        }

        ResetCoroutine();
    }

    // ���� ���Ͽ� ���� �ݺ��ؼ� �����
    public IEnumerator CoLoopMakeDonut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _makingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return new WaitUntil(() => !IsWorking);

        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        AddToPile(go);

        yield return new WaitUntil(() => !IsFull);

        StartCoroutine(CoLoopMakeDonut());
    }

    #endregion

    #region Coroutine

    // ���� ���Ͽ��� ���� ������
    private IEnumerator CoGetFromPile(CharacterBase character)
    {
        if (!CheckPileDonutExist())
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < _gettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject donut = RemoveFromPile();
        FluxSystem.Dispatch(new OnGetItem(EItemType.Donut, donut, character));
    }

    // ���� ���Ͽ� ���� �ױ�
    private IEnumerator CoMoveToPile(CharacterBase character)
    {
        if (!CheckCharacterDonutExist(character))
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < _gettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        FluxSystem.Dispatch(new OnPutDownItemToPile(EItemType.Donut, character, this));
    }

    #endregion

    private IEnumerator CoWaitOtherWorking()
    {
        yield return new WaitUntil(() => !IsWorking);
    }

    private bool CheckCharacterDonutExist(CharacterBase character)
    {
        if (character is CharacterPlayer player)
        {
            return GameManager.GetGameManager.Player.Character.Stock.GetItemByType(EItemType.Donut).Count > 0;
        }
        else if (character is CharacterWorker worker)
        {
            return worker.Stock.GetItemByType(EItemType.Donut).Count > 0;
        }
        else if (character is CharacterCustomer customer)
        {
            return customer.Stock.GetItemByType(EItemType.Donut).Count > 0;
        }

        return false;
    }

    private void ResetCoroutine()
    {
        IsWorking = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }

    private bool CheckPileDonutExist()
    {
        return !IsEmpty;
    }
}
