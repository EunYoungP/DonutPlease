using DonutPlease.Game.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static CharacterCustomerController;

public class CharacterWorkerController : CharacterBase
{
    public enum EWorkerState
    {
        MakeDonut,
        Cashier,
        ClearTrash,
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    public bool IsMoving => CheckMoving();

    public EWorkerState State { get; private set; } = EWorkerState.MakeDonut;

    private void Update()
    {
        if (IsMoving)
        {
            Walk();
        }
        else
        {
            Idle();
        }
    }

    public void ChangeState(EWorkerState state)
    {
        switch (state)
        {
            case EWorkerState.MakeDonut:
                State = EWorkerState.MakeDonut;
                break;
            case EWorkerState.Cashier:
                State = EWorkerState.Cashier;
                break;
            case EWorkerState.ClearTrash:
                State = EWorkerState.ClearTrash;
                break;
        }
    }

    // �ӽ� ��ġ
    private IEnumerator CoWorkerDo()
    {
        // ������ ���� ����� 
        while(true)
        {
            // ���� ��� �������� �˻�
            if(ShouldDoCarryDonut())
            {

            }

            // ĳ�� ���� �����ϴ��� �˻�
            if(ShouldDoMainCounbterCashierJob())
            {

            }

            // ���̺� û�� ���� �����ϴ��� �˻�
            if (ShouldDoClearTrash())
            {

            }
        }
    }

    private bool ShouldDoCarryDonut()
    {
        // ���� �ӽſ� ������ ������

        return false;
    }

    private bool ShouldDoMainCounbterCashierJob()
    {
        // ī���Ϳ� ������ ������

        // ī���Ϳ� �մ��� ��� ����

        // ���̺� �� �ڸ��� ������
        return false;
    }


    private bool ShouldDoClearTrash()
    {
        // �����Ⱑ �ִ� ���̺��� ������

        return false;
    }

    #region Movement

    public void MoveTo(Transform dest)
    {
        _agent.SetDestination(dest.position);
    }

    private bool CheckMoving()
    {
        bool isMoving = !_agent.pathPending
            && _agent.remainingDistance > _agent.stoppingDistance
            && _agent.velocity.magnitude > 0.1f;

        return isMoving;
    }

    #endregion

    #region Animation

    private void Walk()
    {
        _animator.Play(PlayerAnimationNames.Walk);
    }

    private void Idle()
    {
        _animator.Play(PlayerAnimationNames.Idle);
    }

    #endregion
}
