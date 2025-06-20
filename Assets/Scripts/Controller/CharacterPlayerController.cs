using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;

    private bl_Joystick _joystick;
    
    private float _cameraRot = -45f;
    private float _moveSpeed = 0f;
    private float _rotateSpeed = 10f;

    private bool _isMoving;

    public void Initialize()
    {
        _joystick = GameManager.GetGameManager.Joystick;
        _moveSpeed = GameManager.GetGameManager.Player.Character.Stat.MoveSpeed;
    }

    private void Update()
    {
        if (_joystick.gameObject.activeSelf == false)
            return;

        float v = _joystick.Vertical;
        float h = _joystick.Horizontal;

        Quaternion cameraRot = Quaternion.Euler(0, _cameraRot, 0);
        Vector3 dir = cameraRot * new Vector3(h, 0, v);

        _isMoving = dir != Vector3.zero;
        if (_isMoving)
        {
            Walk();
        }
        else
        {
            Idle();
        }

        if (GameManager.GetGameManager.Tutorial.IsTutorial)
            return;

        Move(dir);
        Rotate(dir);
    }


    #region Movement
    private void Move(Vector3 dir)
    {
        Vector3 moveOffset = dir * _moveSpeed * Time.deltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }

    // Rotate
    private void Rotate(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
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
