using UnityEngine;

using UnityEngine.TextCore.Text;

namespace DonutPlease.Game.Character
{
    public class CharacterPlayer : CharacterBase
    {
        [SerializeField] private CharacterPlayerController _controller;
        [SerializeField] private TrayController _trayController;

        private PlayerCamera _camera;

        public CharacterPlayerController Controller => _controller;

        public void Initialize()
        {
            _camera = Camera.main.GetComponent<PlayerCamera>();
            _camera.Initialize(this);
        }

        #region Tray

        protected override void AddToTray(CharacterBase player, Transform child)
        {
            GameMng.Player.Stock.AddDonut(child.gameObject);
            _trayController.PlayAddToTray(child);
        }

        protected override void RemoveFromTray(CharacterBase character, PileBase pile)
        {
            if (character is CharacterPlayer player)
            {
                var donut = GameMng.Player.Stock.RemoveDonut();
                _trayController.PlayPutDownFromTray(donut.transform, pile);
            }
        }

        #endregion
    }
}

