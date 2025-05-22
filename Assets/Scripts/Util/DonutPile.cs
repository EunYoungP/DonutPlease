using DonutPlease.Game.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class DonutPile : PileBase
{
    [SerializeField] private GameObject _prefab;

    private Coroutine _curCoroutine;

    private CharacterBase _enterCharcater;

    public float MakingInterval { get; private set; } = 0.2f;
    public float GettingInterval { get; private set; } = 0.2f;
    public bool IsGettingDonut { get; private set; } = false;
    public bool IsTakingDonut { get; private set; } = false;
    public bool IsWorkingAI { get; private set; } = false;

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    public void OnTriggerEnterGetDonut(CharacterBase character)
    {
        _enterCharcater = character;

        ResetCoroutine();

        IsGettingDonut = true;

        StartCoroutine(CoLoopEnterGetCoroutine());
    }

    public void OnTriggerEnterTakeDonut(CharacterBase character)
    {
        _enterCharcater = character;

        ResetCoroutine();

        IsTakingDonut = true;

        StartCoroutine(CoLoopEnterTakeCoroutine());
    }

    public void OnTriggerExitGetDonut()
    {
        _enterCharcater = null;

        ResetCoroutine();
    }

    public void GetDonutFromPile(int count)
    {
        IsWorkingAI = true;

        StartCoroutine(CoGetFromPileByCount(count));
    }

    private void ResetCoroutine()
    {
        IsGettingDonut = false;
        IsTakingDonut = false;

        if (_curCoroutine != null)
            StopCoroutine(_curCoroutine);
    }

    private IEnumerator CoLoopEnterGetCoroutine()
    {
        while (IsGettingDonut)
        {
            yield return new WaitUntil(() => !IsWorkingAI);

            _curCoroutine = StartCoroutine(CoGetFromPile());
            yield return _curCoroutine;
        }
        yield return null;
    }

    private IEnumerator CoLoopEnterTakeCoroutine()
    {
        while (IsTakingDonut)
        {
            yield return new WaitUntil(() => !IsWorkingAI);

            _curCoroutine = StartCoroutine(CoMoveToPile());
            yield return _curCoroutine;
        }
        yield return null;
    }

    private IEnumerator CoGetFromPileByCount(int count)
    {
        int getDonutCount = 0;
        while (IsWorkingAI && getDonutCount < count)
        {
            yield return StartCoroutine(CoGetFromPile());
            getDonutCount++;
        }

        IsWorkingAI = false;
    }

    // µµ³Ó ÆÄÀÏ¿¡¼­ µµ³Ó »©°¡±â
    private IEnumerator CoGetFromPile()
    {
        if (!CheckPileDonutExist())
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < GettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject donut = RemoveFromPile();
        FluxSystem.Dispatch(new OnGetDonut(donut, _enterCharcater));
    }

    // µµ³Ó ÆÄÀÏ¿¡ µµ³Ó ½×±â
    private IEnumerator CoMoveToPile()
    {
        if (!CheckCharacterDonutExist())
            yield break;

        float elapsedTime = 0f;
        while (elapsedTime < GettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        FluxSystem.Dispatch(new OnPutDownDonut(_enterCharcater, this));
    }

    // µµ³Ó ÆÄÀÏ¿¡ µµ³Ó ¹Ýº¹ÇØ¼­ ¸¸µé±â
    private IEnumerator CoLoopMakeDonut()
    {
        yield return new WaitUntil(() => !IsWorkingAI);

        float elapsedTime = 0f;

        while (elapsedTime < MakingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (IsGettingDonut)
            yield break;

        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        AddToPile(go);

        if (!IsFull)
            StartCoroutine(CoLoopMakeDonut());
    }

    private bool CheckCharacterDonutExist()
    {
        if (_enterCharcater is CharacterPlayer player)
        {
            return GameManager.GetGameManager.Player.Stock.DonutCount > 0;
        }
        else if (_enterCharcater is CharacterCustomer customer)
        {

        }

        return false;
    }

    private bool CheckPileDonutExist()
    {
        return !IsEmpty;
    }
}
