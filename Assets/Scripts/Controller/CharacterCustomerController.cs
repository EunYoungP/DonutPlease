using UnityEngine;
using UnityEngine.AI;

public class CharacterCustomerController : MonoBehaviour
{
    public enum ECustomerState
    { 
        None,
        In,
        Waiting,
        EatDonut,
        Out,
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    public bool IsMoving => CheckMoving();

    public ECustomerState State { get; private set; } = ECustomerState.In;


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

    public void ChangeState(ECustomerState state)
    {
        switch (state)
        {
            case ECustomerState.In:
                State = ECustomerState.In;
                break;
            case ECustomerState.Waiting:
                State = ECustomerState.Waiting;
                break;
            case ECustomerState.EatDonut:
                State = ECustomerState.EatDonut;
                break;
            case ECustomerState.Out:
                State = ECustomerState.Out;
                break;
        }
    }

    public bool CheckState(ECustomerState state)
    {
        return State == state;
    }

    #region Movement

    public void MoveTo(Vector3 dest)
    {
        _agent.SetDestination(dest);
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
