using UnityEngine;

public class CharacterCarController : MonoBehaviour
{
    public enum ECarState
    {
        None,
        In,
        Waiting,
        Out,
    }

    public ECarState State { get; private set; } = ECarState.In;

    public void ChangeState(ECarState state)
    {
        switch (state)
        {
            case ECarState.In:
                State = ECarState.In;
                break;
            case ECarState.Waiting:
                State = ECarState.Waiting;
                break;
                break;
            case ECarState.Out:
                State = ECarState.Out;
                break;
        }
    }

    public bool CheckState(ECarState state)
    {
        return State == state;
    }
}
