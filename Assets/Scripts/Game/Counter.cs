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
        // �մ� �����ϴ� �ڵ� �� ����
        AddCustomer(customer);

        while (true)
        {
            // ���� ������ ���ͼ� �ټ���
            AddCustomerInLine(customer);
            customer.Controller.MoveTo(GetLinePos());

            // �̵���
            yield return new WaitUntil(() => customer.Controller.IsMoving);

            // ���
            customer.Controller.ChangeState(CharacterCustomerController.ECustomerState.Waiting);

            // ���� ���� �������� �˻�? ���?
            if (CanGetDonut())
            {
                // �ٿ��� ������
                RemoveCustomerInLine();

                // ���� �ޱ�

                // �ڸ��� �̵�
                //customer.Controller.MoveTo(�� �ڸ�)

                // ���� �� ����
                UpdateCustomerLine();

                // �������� ���

                // ���� �� �Ա�

                // ���� �� ������
            }
        }
    }

    private void UpdateCustomerLine()
    {
        foreach (var customer in _customersInLine)
        {
            // ��� ���� �մ��� ���� ���� �ڵ�
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
        // ������ �ִ��� �˻�

        // �� �ڸ��� �ִ��� �˻�

        return false;
    }

    // �մ� ���� �� üũ
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
