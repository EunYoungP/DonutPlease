using UnityEngine;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private PileBase _moneyPile;

    [SerializeField]
    private Collider _casherPlaceCollider;

    private Queue<CharacterCustomer> _customers = new();

    private void Initialize()
    {
        FluxSystem.ColliderActionStream.Subscribe(data =>
        {
            OnTriggerAction(data.Item1, data.Item2);
        }).AddTo(this);
    }

    private void OnTriggerAction(CharacterBase target, EColliderIdentifier identifier)
    {
        switch (identifier)
        {
            case EColliderIdentifier.Donut:
                // GetBurger
                break;
            case EColliderIdentifier.Cash:
                // TakeCash
                break;
            case EColliderIdentifier.CasherPlace:
                // Payment
                break;
        }
    }

    private void GetDonut(CharacterBase target)
    {
        // ������ �÷��̾�� ������ ���̺�� �Ѿ�� �ִϸ��̼ǵ� ���.
        // => �켱 �÷��̾ ������ ���� �ӽſ��� �����ͼ� ����ִ� �κ� �����ؾ���.
        // ���� �÷��̾� �����Ϳ��� ī���ͷ� ���� ������ �߰� �Ǿ����.
        _donutPile.AddToPile(target);
    }

    private void TakeCash()
    {
    }

    private void Payment()
    {

    }
}
