using DonutPlease.Game.Character;
using System.Collections;
using UnityEngine;

public class DonutPile : PileBase
{
    [SerializeField] private GameObject _prefab;

    public float MakingInterval { get; private set; } = 0.2f;
    public float GettingInterval { get; private set; } = 0.2f;
    public bool IsGettingDonut { get; private set; } = false;

    private CharacterBase _enterCharcater;

    public void OnTriggerEnterGetDonut(CharacterBase character)
    {
        _enterCharcater = character;

        IsGettingDonut = true;

        StartCoroutine(CoGetFromPile());
    }

    public void OnTriggerEnterTakeDonut(CharacterBase character)
    {
        _enterCharcater = character;

        IsGettingDonut = true;

        StartCoroutine(CoMoveToPile());
    }

    public void OnTriggerExitGetDonut()
    {
        _enterCharcater = null;

        IsGettingDonut = false;

        StopCoroutine(CoMoveToPile());
    }

    public void AddToStock(GameObject donut)
    {
        if (_enterCharcater is CharacterCustomer customer)
        {
            //customer.SetDonut(go);
        }

        if (_enterCharcater is CharacterPlayer player)
        {
            player.PlayerStock.AddDonut(donut);
        }
    }

    // µµ³Ó ÆÄÀÏ¿¡¼­ µµ³Ó »©°¡±â
    private IEnumerator CoGetFromPile()
    {
        float elapsedTime = 0f;
        while (elapsedTime < GettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        GameObject donut = RemoveFromPile();
        FluxSystem.Dispatch(new OnGetDonut(donut, _enterCharcater));

        StartCoroutine(CoGetFromPile());
    }

    private IEnumerator CoMoveToPile()
    {
        float elapsedTime = 0f;
        while (elapsedTime < GettingInterval)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        FluxSystem.Dispatch(new OnPutDownDonut(_enterCharcater));

        StartCoroutine(CoMoveToPile());
    }

    // µµ³Ó ÆÄÀÏ¿¡ µµ³Ó ¸¸µé±â
    private IEnumerator CoMakeDonut()
    {
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

        Debug.Log("µµ³Ó ¸¸µé±â");

        if (!IsFull)
            StartCoroutine(CoMakeDonut());
    }
}
