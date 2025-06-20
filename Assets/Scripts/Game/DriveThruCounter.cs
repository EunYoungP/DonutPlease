using DG.Tweening;
using DonutPlease.Game.Character;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class DriveThruCounter : MonoBehaviour
{
    [SerializeField]
    private DonutPile _boxPile; // ���� �ڽ� ����

    [SerializeField]
    private Transform _boxPileFrontPos;

    [SerializeField]
    private CashPile _cashPile;

    [SerializeField]
    private Transform _cashierPlace;

    [SerializeField]
    private List<Transform> _carPoses;

    [Header("Customer")]
    [SerializeField] GameObject Customer;
    [SerializeField] private Transform _customerInPos;
    [SerializeField] private Transform _customerOutPos;

    private List<CharacterCar> _customers = new();
    private List<CharacterCar> _customersInLine = new(); // �ڵ��� �ٿ� �ʿ��ұ�?
    private CharacterBase _cashier = null;
    private const int _inLineCustomerMax = 6;

    public int DonutCount => _boxPile.ObjectCount;
    public int CustomerCount => _customers.Count;
    public int InLineCustomerCount => _customersInLine.Count;
    public bool HaveCashier => _cashier != null;
    public DonutPile DonutPile => _boxPile;
    public Transform DonutPileFrontPosition => _boxPileFrontPos;
    public Transform CashierPlace => _cashierPlace;

    public void Initialize()
    {
        FluxSystem.ColliderTriggerActionStream.Subscribe(data =>
        {
            if (data is FxOnTriggerEnter fxTriggerEnter)
            {
                SetCashier(fxTriggerEnter.characterBase);

                OnTriggerEnterAction(fxTriggerEnter.characterBase, fxTriggerEnter.colliderType);
            }

            if (data is FxOnTriggerExit fxTriggerExit)
            {
                SetCashier(null);

                OnTriggerExitAction(fxTriggerExit.characterBase, fxTriggerExit.colliderType);
            }
        }).AddTo(this);

        StartCoroutine(CoAddCustomer());
    }

    public void SetCashier(CharacterBase cashier)
    {
        _cashier = cashier;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator CoAddCustomer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_inLineCustomerMax > InLineCustomerCount)
            {
                AddCustomer();

                // ���� 1. �մ� ����;
                var customer = _customers[CustomerCount - 1];

                StartCoroutine(CoCustomerDo(customer));

                yield return new WaitForSeconds(10f);
            }
            yield return null;
        }
    }

    private IEnumerator CoCustomerDo(CharacterCar customer)
    {
        while (true)
        {
            if (customer.Controller.CheckState(CharacterCarController.ECarState.In))
            {
                // 1. �ٿ� �߰�
                AddCustomerInLine(customer);

                Debug.Log("���� 2. �ٱ��� �̵� ���� �� ���");
                var index = _customersInLine.IndexOf(customer);
                for(int i = _carPoses.Count - 1; i >= index; i--)
                {
                    var carPose = _carPoses[i];

                    yield return customer.transform.DOMove(carPose.position, 0.5f).WaitForCompletion();
                    yield return customer.transform.DORotateQuaternion(carPose.rotation, 1f).WaitForCompletion();
                }

                // ��� ���·� ����
                Debug.Log("���� 3. ��� ���·� ����");
                customer.Controller.ChangeState(CharacterCarController.ECarState.Waiting);
            }

            if (customer.Controller.CheckState(CharacterCarController.ECarState.Waiting))
            {
                Debug.Log("���� 4. ���� ���� �˻�");
                if (CanGetDonut(customer))
                {
                    Debug.Log("���� 5. ���� �ޱ�");
                    const int DonutCount = 1;

                    // ���� �ޱ�
                    _boxPile.GetDonutFromPileByCount(customer, DonutCount);

                    // �� ����
                    customer.Pay(DonutCount, _cashPile);

                    Debug.Log("���� 6-1. ���� �ޱ� ���");
                    yield return new WaitUntil(() => !_boxPile.IsWorking);

                    // �ٿ��� ����
                    RemoveCustomerInLine();

                    // ���� �� ����
                    Debug.Log("���� 8. ������ �մ� �� ���� ����");
                    UpdateCustomerLine();

                    customer.Controller.ChangeState(CharacterCarController.ECarState.Out);
                }
            }

            if (customer.Controller.CheckState(CharacterCarController.ECarState.Out))
            {
                // - ������
                Debug.Log("���� 12. �� ������ �̵����� �� ���");
                yield return customer.transform.DOMove(_customerOutPos.position, 2f).WaitForCompletion();

                Debug.Log("���� 14. �մ� ����");
                RemoveCustomer();

                yield break;
            }

            yield return null;
        }
    }

    private bool CanGetDonut(CharacterCar customer)
    {
        // ������ �ִ��� �˻�
        if (_boxPile.IsEmpty)
            return false;

        if (!HaveCashier)
            return false;

        if (customer != _customersInLine.First())
            return false;

        return true;
    }

    private void AddCustomer()
    {
        GameObject customerObj = Instantiate(Customer, _customerInPos.position, Quaternion.identity);
        CharacterCar customer = customerObj.GetComponent<CharacterCar>();

        _customers.Add(customer);
    }

    private void RemoveCustomer()
    {
        var customer = _customers[0];
        _customers.Remove(customer);
        Destroy(customer.gameObject);
    }

    private void AddCustomerInLine(CharacterCar customer)
    {
        _customersInLine.Add(customer);
    }

    private bool RemoveCustomerInLine()
    {
        var customer = _customersInLine[0];
        return _customersInLine.Remove(customer);
    }

    private void UpdateCustomerLine()
    {
        foreach (var customer in _customersInLine)
        {
            customer.transform.DOMove(GetLinePos(customer).position, 2f)
                    .OnComplete(() => { customer.transform.DORotateQuaternion(GetLinePos(customer).rotation, 2f).WaitForCompletion();});
        }
    }

    private Transform GetLinePos(CharacterCar customer)
    {
        int index = _customersInLine.IndexOf(customer);
        return _carPoses[index].transform;
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
            case EColliderIdentifier.CashierPlace:
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
            case EColliderIdentifier.CashierPlace:
                // Payment
                break;
        }
    }

    private void StartGetDonut(CharacterBase target)
    {
        _boxPile.OnTriggerEnterGetDonut(target);
    }

    private void StartTakeDonut(CharacterBase target)
    {
        _boxPile.OnTriggerEnterTakeDonut(target);
    }

    private void StopGetDonut()
    {
        _boxPile.OnTriggerExitGetDonut();
    }


    #endregion
}
