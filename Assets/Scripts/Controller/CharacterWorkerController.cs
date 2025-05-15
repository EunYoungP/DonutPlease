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

    // 임시 위치
    private IEnumerator CoWorkerDo()
    {
        // 시작은 도넛 만들기 
        while(true)
        {
            // 도넛 운반 가능한지 검사
            if(ShouldDoCarryDonut())
            {

            }

            // 캐셔 업무 존재하는지 검사
            if(ShouldDoMainCounbterCashierJob())
            {

            }

            // 테이블 청소 업무 존재하는지 검사
            if (ShouldDoClearTrash())
            {

            }
        }
    }

    private bool ShouldDoCarryDonut()
    {
        // 도넛 머신에 도넛이 존재함

        return false;
    }

    private bool ShouldDoMainCounbterCashierJob()
    {
        // 카운터에 도넛이 존재함

        // 카운터에 손님이 대기 중임

        // 테이블에 빈 자리가 존재함
        return false;
    }


    private bool ShouldDoClearTrash()
    {
        // 쓰레기가 있는 테이블이 존재함

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
