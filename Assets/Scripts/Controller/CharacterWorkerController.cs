using DonutPlease.Game.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static CharacterCustomerController;

public class CharacterWorkerController : CharacterBase
{
    public enum EWorkerState
    {
        CarryDonut,
        Cashier,
        ClearTrash,
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    public bool IsMoving => CheckMoving();

    public EWorkerState State { get; private set; } = EWorkerState.CarryDonut;

    private void Update()
    {
        UpdateAnimation();
    }

    public void ChangeState(EWorkerState state)
    {
        switch (state)
        {
            case EWorkerState.CarryDonut:
                State = EWorkerState.CarryDonut;
                break;
            case EWorkerState.Cashier:
                State = EWorkerState.Cashier;
                break;
            case EWorkerState.ClearTrash:
                State = EWorkerState.ClearTrash;
                break;
        }
    }


    #region Movement

    public void MoveTo(Transform dest)
    {
        _agent.SetDestination(dest.position);
    }

    private bool CheckMoving()
    {
        if (_agent.pathPending)
            return true;

        if (_agent.hasPath == false)
            return false;

        if (_agent.remainingDistance <= _agent.stoppingDistance
                      && _agent.velocity.magnitude < 0.1f)
            return false;

        return true;
    }

    public Vector3 GetVelocity()
    {
        return _agent.velocity.normalized;
    }

    #endregion

    #region Animation

    private void UpdateAnimation()
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
