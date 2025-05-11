using UnityEngine;
using UniRx;

namespace DonutPlease.Game.Character
{
    public class CharacterPlayer : CharacterBase
    {
        GameManager _gameManager;

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
            _gameManager = GameManager.GetGameManager;
            _joystick = GameManager.GetGameManager.JoyStick;

            _camera = Camera.main.GetComponent<PlayerCamera>();
            _camera.Initialize(this);

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnGetDonut getDonut)
                {
                    AddToTray(getDonut.donut.transform);
                }
            })
            .AddTo(this);

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnPutDownDonut putDownDonut)
                {
                    if (putDownDonut.character is CharacterPlayer player)
                        RemoveFromTray();
                }
            })
            .AddTo(this);
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

        #region Tray

        public void AddToTray(Transform child)
        {
            _gameManager.Player.Stock.AddDonut(child.gameObject);
            _trayController.PlayAddToTray(child);
        }

        public void RemoveFromTray()
        {
            var donut = _gameManager.Player.Stock.RemoveDonut();
            _trayController.PlayPutDownFromTray(donut.transform);
        }

        #endregion

        #region Movement
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
}

