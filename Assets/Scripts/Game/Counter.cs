using UnityEngine;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;
using System.Collections;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private PileBase _moneyPile;

    [SerializeField]
    private Collider _casherPlaceCollider;

    private Queue<CharacterCustomer> _customers = new();
    private Queue<CharacterCustomer> _customersInLine = new();
    private int _inLineCustomerMax = 10;

    //Line
    [SerializeField]
    private Transform _lineStartTransform;

    public int CustomerCount => _customers.Count;
    public int InLineCustomerCount => _customersInLine.Count;

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

    #region customer

    private IEnumerator CoCustomerDo(CharacterCustomer customer)
    {
        // 손님 생성하는 코드 후 실행
        AddCustomer(customer);

        while (true)
        {
            // 가게 안으로 들어와서 줄서기
            AddCustomerInLine(customer);
            customer.Controller.MoveTo(GetLinePos());

            // 이동중
            yield return new WaitUntil(() => customer.Controller.IsMoving);

            // 대기
            customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Waiting);

            // 도넛 받을 차례인지 검사? 대기?
            if (CanGetDonut())
            {
                // 줄에서 나가기
                RemoveCustomerInLine();

                // 도넛 받기

                // 자리로 이동
                //customer.Controller.MoveTo(빈 자리)

                // 도넛 줄 갱신
                UpdateCustomerLine();

                // 도착까지 대기

                // 도착 후 먹기

                // 먹은 후 나가기
            }
        }
    }

    private void UpdateCustomerLine()
    {
        foreach (var customer in _customersInLine)
        {
            // 대기 중인 손님이 줄을 서는 코드
            customer.Controller.MoveTo(GetLinePos());
        }
    }

    private Transform GetLinePos()
    {
        Transform lineTransform = _lineStartTransform;
        lineTransform.position += new Vector3(0, 0, InLineCustomerCount - 1);
        return lineTransform;
    }

    private bool CanGetDonut()
    {
        // 도넛이 있는지 검사

        // 빈 자리가 있는지 검사

        return false;
    }

    // 손님 생성 전 체크
    private bool CanAddCustomer()
    {
        return InLineCustomerCount < _inLineCustomerMax;
    }

    private void AddCustomer(CharacterCustomer customer)
    {
        _customers.Enqueue(customer);
    }

    private bool RemoveCustomer()
    {
        return (_customers.TryDequeue(out CharacterCustomer result));
    }

    private void AddCustomerInLine(CharacterCustomer customer)
    {
        _customersInLine.Enqueue(customer);
    }

    private bool RemoveCustomerInLine()
    {
        return (_customersInLine.TryDequeue(out CharacterCustomer result));
    }


    #endregion

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
            case EColliderIdentifier.TakeDonut:
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
