using UnityEngine;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private PileBase _moneyPile;

    [SerializeField]
    private Collider _casherPlaceCollider;

    private Queue<CharacterCustomer> _customers = new();

    private void OnEnable()
    {
        FluxSystem.ColliderEnterActionStream.Subscribe(data =>
        {
            OnTriggerEnterAction(data.Item1, data.Item2);

        }).AddTo(this);

        FluxSystem.ColliderExitActionStream.Subscribe(data =>
        {
            OnTriggerExitAction(data.Item1, data.Item2);

        }).AddTo(this);
    }

    #region trigger

    private void OnTriggerEnterAction(CharacterBase target, EColliderIdentifier identifier)
    {
        switch (identifier)
        {
            case EColliderIdentifier.GetDonut:
                StartGetDonut(target);
                break;
            case EColliderIdentifier.TakeDonut:
                StartTakeDonut(target);
                break;
            case EColliderIdentifier.Cash:
                // TakeCash
                break;
            case EColliderIdentifier.CasherPlace:
                // Payment
                break;
        }
    }

    private void OnTriggerExitAction(CharacterBase target, EColliderIdentifier identifier)
    {
        switch (identifier)
        {
            case EColliderIdentifier.GetDonut:
                StopGetDonut();
                break;
            case EColliderIdentifier.Cash:
                // TakeCash
                break;
            case EColliderIdentifier.CasherPlace:
                // Payment
                break;
        }
    }

    private void StartGetDonut(CharacterBase target)
    {
        _donutPile.OnTriggerEnterGetDonut(target);
    }

    private void StartTakeDonut(CharacterBase target)
    {
        _donutPile.OnTriggerEnterTakeDonut(target);
    }

    private void StopGetDonut()
    {
        _donutPile.OnTriggerExitGetDonut();
    }

    private void TakeCash()
    {
    }

    private void Payment()
    {

    }

    #endregion
}
