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
        // 실제로 플레이어에서 도넛이 테이블로 넘어가는 애니메이션도 재생.
        // => 우선 플레이어가 도넛을 도넛 머신에서 가져와서 들고있는 부분 구현해야함.
        // 실제 플레이어 데이터에서 카운터로 도넛 개수가 추가 되어야함.
        _donutPile.AddToPile(target);
    }

    private void TakeCash()
    {
    }

    private void Payment()
    {

    }
}
