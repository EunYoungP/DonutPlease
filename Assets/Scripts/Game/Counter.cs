using UnityEngine;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private PileBase _moneyPile;

    [SerializeField]
    private Collider _casherPlaceCollider;

    [Header("Customer")]
    [SerializeField] GameObject Customer;

    [SerializeField]
    private Transform _lineStartTransform;

    [SerializeField]
    private Transform _customerPoint;

    private List<CharacterCustomer> _customers = new();
    private List<CharacterCustomer> _customersInLine = new();
    private int _inLineCustomerMax = 1;

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

        StartCoroutine(CoAddCustomer());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region customer

    private IEnumerator CoAddCustomer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if (_inLineCustomerMax > InLineCustomerCount)
            {
                AddCustomer();
                Debug.Log("은영 1. 손님 생성");

                var customer = _customers[CustomerCount - 1];

                StartCoroutine(CoCustomerDo(customer));

                yield return new WaitForSeconds(10f);
            }
            yield return null;
        }
    }

    private IEnumerator CoCustomerDo(CharacterCustomer customer)
    {
        while (true)
        {
            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.None))
            {
                customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.In);

                // 가게 안으로 들어와서 줄서기
                AddCustomerInLine(customer);

                Debug.Log("은영 2. 줄까지 이동 실행");
                customer.Controller.MoveTo(GetLinePos(customer));

                // 이동중
                Debug.Log("은영 3. 줄까지 이동중");
                yield return new WaitUntil(() => !customer.Controller.IsMoving);

                // 대기 상태로 변경
                Debug.Log("은영 4. 대기 상태로 변경");
                customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Waiting);
            }

            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.Waiting))
            {
                Debug.Log("은영 5. 도넛 차례 검사");
                if (CanGetDonut())
                {
                    customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.EatDonut);

                    // 줄에서 나가기
                    RemoveCustomerInLine();

                    Debug.Log("은영 6. 도넛 받기");
                    // 도넛 받기
                    _donutPile.GetDonutFromPile(customer, 1);

                    yield return new WaitForSeconds(1f);

                    Debug.Log("은영 6-1. 도넛 받기 대기");
                    yield return new WaitUntil(() => !_donutPile.IsWorkingAI);

                    // 자리로 이동
                    Vector3 emptySeatPos = GameManager.GetGameManager.Store.GetStore(1).GetEmptyTableSeat(out var table);
                    Debug.Log($"은영 7. 자리로 이동 실행");
                    Debug.Log($"은영 7. POS X : {emptySeatPos.x}, Y : {emptySeatPos.y} Z : {emptySeatPos.z}");
                    customer.Controller.MoveTo(emptySeatPos);

                    // 도넛 줄 갱신
                    Debug.Log("은영 8. 나머지 손님 줄 갱신 실행");
                    UpdateCustomerLine();

                    // 도착까지 대기
                    Debug.Log("은영 9. 도착까지 대기");
                    yield return new WaitUntil(() => !customer.Controller.IsMoving);

                    // 도착 후 
                    // - 앉기

                    // - 도넛 내려놓기
                    _donutPile.MoveDonutToPile(customer, 1);

                    // - 먹기
                    Debug.Log("은영 10. 먹는동안 5초 대기");
                    yield return new WaitForSeconds(10f);

                    // 먹은 후 나가기
                    // - 손님 도넛 삭제

                    // - 쓰레기 생성
                    Debug.Log("은영 11. 쓰레기 생성");
                    table.MakeTrash();

                    customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Out);
                }
            }

            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.Out))
            { 
                // - 나가기
                Debug.Log("은영 12. 문 밖으로 이동실행");
                customer.Controller.MoveTo(_customerPoint.position);

                Debug.Log("은영 13. 문 밖으로 이동중");
                yield return new WaitUntil(() => !customer.Controller.IsMoving);

                Debug.Log("은영 14. 손님 삭제");
                RemoveCustomer();

                yield break;
            }
            
            yield return null;
          }
    }

    private void UpdateCustomerLine()
    {
        foreach (var customer in _customersInLine)
        {
            // 대기 중인 손님이 줄을 서는 코드
            customer.Controller.MoveTo(GetLinePos(customer));
        }
    }

    private Vector3 GetLinePos(CharacterCustomer customer)
    {
        int index = _customersInLine.IndexOf(customer);

        Vector3 lineStartPos = _lineStartTransform.position;
        lineStartPos += new Vector3(index * 1.2f, 0, 0 );
        return lineStartPos;
    }

    private bool CanGetDonut()
    {
        // 도넛이 있는지 검사
        if (_donutPile.IsEmpty)
            return false;

        // 빈 자리가 있는지 검사
        if (!GameManager.GetGameManager.Store.GetStore(1).CheckHaveEmptySeat())
        {
            return false;
        }

        return true;
    }

    // 손님 생성 전 체크
    private bool CanAddCustomer()
    {
        return InLineCustomerCount < _inLineCustomerMax;
    }

    private void AddCustomer()
    {
        GameObject customerObj = Instantiate(Customer, _customerPoint.position, Quaternion.identity);
        CharacterCustomer customer = customerObj.GetComponent<CharacterCustomer>();

        _customers.Add(customer);
    }

    private void RemoveCustomer()
    {
        var customer = _customers[0];
        _customers.Remove(customer);
        Destroy(customer.gameObject);
    }

    private void AddCustomerInLine(CharacterCustomer customer)
    {
        _customersInLine.Add(customer);
    }

    private bool RemoveCustomerInLine()
    {
        var customer = _customersInLine[0];
        return _customersInLine.Remove(customer);
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
