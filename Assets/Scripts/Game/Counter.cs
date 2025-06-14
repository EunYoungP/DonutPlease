using UnityEngine;
using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;
using System;

public class Counter : PropBase
{
    [SerializeField]
    private DonutPile _donutPile;

    [SerializeField]
    private CashPile _cashPile;

    [SerializeField]
    private Transform _donutPileFrontPos;

    [SerializeField]
    private Transform _casherPlace;

    [Header("Customer")]
    [SerializeField] GameObject Customer;
    [SerializeField] private Transform _lineStartTransform;
    [SerializeField] private Transform _customerPoint;

    private List<CharacterCustomer> _customers = new();
    private List<CharacterCustomer> _customersInLine = new();
    private const int _inLineCustomerMax = 2;

    private CharacterBase _cashier = null;

    public int DonutCount => _donutPile.ObjectCount;
    public int CustomerCount => _customers.Count;
    public int InLineCustomerCount => _customersInLine.Count;
    public bool HaveCashier => _cashier != null;
    public DonutPile DonutPile => _donutPile;

    public Transform DonutPileFrontPosition => _donutPileFrontPos;
    public Transform CasherPlace => _casherPlace;

    private void OnEnable()
    {
        FluxSystem.ColliderEnterActionStream.Subscribe(data =>
        {
            SetCashier(data.Item1);

            OnTriggerEnterAction(data.Item1, data.Item2);

        }).AddTo(this);

        FluxSystem.ColliderExitActionStream.Subscribe(data =>
        {
            SetCashier(null);

            OnTriggerExitAction(data.Item1, data.Item2);

        }).AddTo(this);

        //FluxSystem.ActionStream
        //.Subscribe(data =>
        //{
        //    if (data is OnGetItem onGetCash)
        //    {
        //        if (onGetCash.character is CharacterCustomer customer)
        //        {
        //            // ���� ���ֱ�
        //        }
        //    }
        //}).AddTo();

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
                Debug.Log("���� 1. �մ� ����");

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
            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.In))
            {
                // ���� ������ ���ͼ� �ټ���
                AddCustomerInLine(customer);

                Debug.Log("���� 2. �ٱ��� �̵� ����");
                customer.Controller.MoveTo(GetLinePos(customer));

                // �̵���
                Debug.Log("���� 3. �ٱ��� �̵���");
                yield return new WaitUntil(() => !customer.Controller.IsMoving);

                // ��� ���·� ����
                Debug.Log("���� 4. ��� ���·� ����");
                customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Waiting);
            }

            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.Waiting))
            {
                Debug.Log("���� 5. ���� ���� �˻�");
                if (CanGetDonut())
                {
                    customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.EatDonut);

                    // �ٿ��� ����
                    RemoveCustomerInLine();

                    Debug.Log("���� 6. ���� �ޱ�");

                    const int DonutCount = 1;

                    // ���� �ޱ�
                    _donutPile.GetDonutFromPileByCount(customer, DonutCount);

                    // �� ����
                    customer.Pay(DonutCount, _cashPile);

                    // �� ���ֱ�

                    Debug.Log("���� 6-1. ���� �ޱ� ���");
                    yield return new WaitUntil(() => !_donutPile.IsWorking);

                    // �ڸ��� �̵�
                    var store = GameManager.GetGameManager.Store.GetStore(1);
                    Vector3 emptySeatPos = store.GetEmptyTableSeat(out var table, out var seatIndex);

                    Debug.Log($"���� 7. �ڸ��� �̵� ����");
                    customer.Controller.MoveTo(emptySeatPos);

                    // ���� �� ����
                    Debug.Log("���� 8. ������ �մ� �� ���� ����");
                    UpdateCustomerLine();

                    // �������� ���
                    Debug.Log("���� 9. �������� ���");
                    yield return new WaitUntil(() => !customer.Controller.IsMoving);

                    // ���� �� 
                    // - �ɱ�

                    // - ���� ��������
                    _donutPile.MoveDonutToPile(customer, 1);

                    // - �Ա�
                    Debug.Log("���� 10. �Դµ��� 5�� ���");
                    yield return new WaitForSeconds(10f);

                    // ���� �� ������
                    // - �մ� ���� ����

                    // - ������ ����
                    Debug.Log("���� 11. ������ ����");
                    table.MakeTrash(seatIndex);

                    // �ڸ� ���� ������Ʈ
                    table.UpdateSeatEmptyState(seatIndex);

                    customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Out);
                }
            }

            if (customer.Controller.CheckState(CharacterCustomerController.ECustomerState.Out))
            { 
                // - ������
                Debug.Log("���� 12. �� ������ �̵�����");
                customer.Controller.MoveTo(_customerPoint.position);

                Debug.Log("���� 13. �� ������ �̵���");
                yield return new WaitUntil(() => !customer.Controller.IsMoving);

                Debug.Log("���� 14. �մ� ����");
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
            // ��� ���� �մ��� ���� ���� �ڵ�
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
        // ������ �ִ��� �˻�
        if (_donutPile.IsEmpty)
            return false;

        if (!HaveCashier)
            return false;

        // �� �ڸ��� �ִ��� �˻�
        if (!GameManager.GetGameManager.Store.GetStore(1).CheckHaveEmptySeat())
        {
            return false;
        }

        return true;
    }

    // �մ� ���� �� üũ
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

    // workerAI �� �й�ÿ��� ĳ�� ���� �ʿ�
    public void SetCashier(CharacterBase cashier)
    {
        _cashier = cashier;
    }
}
