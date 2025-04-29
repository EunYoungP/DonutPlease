using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterPlayer : CharacterBase
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;

        [SerializeField] private TrayController _trayController;

        private PlayerCamera _camera;
        private bl_Joystick _joystick;

        private float _cameraRot = -45f;
        private float _moveSpeed = 1.2f;
        private float _rotateSpeed = 10f;

        private bool _isMoving;

        public PlayerStockComponent PlayerStock;

        public void Initialize()
        {
            _joystick = GameManager.GetGameManager.JoyStick;

            _camera = Camera.main.GetComponent<PlayerCamera>();
            _camera.Initialize(this);

            PlayerStock = new PlayerStockComponent();
        }

        private void Update()
        {
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

            Move(dir);
            Rotate(dir);
        }

        private void Move(Vector3 dir)
        {
            Vector3 moveOffset = dir * _moveSpeed *  Time.deltaTime;
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
}

