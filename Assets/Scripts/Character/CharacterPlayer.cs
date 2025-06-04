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

        public CharacterStockComponent Stock { get; private set; }

        public void Initialize()
        {
            Stock = new CharacterStockComponent();

            _camera = Camera.main.GetComponent<PlayerCamera>();
            _camera.Initialize(this);
        }

        #region Tray

        protected override void AddToTray(CharacterBase player, Transform child)
        {
            Stock.AddDonut(child.gameObject);

            _trayController.PlayAddToTray(child);
        }

        protected override void RemoveFromTray(CharacterBase character, PileBase pile)
        {
            if (character is CharacterPlayer player)
            {
                var donut = Stock.RemoveDonut();
                _trayController.PlayPutDownFromTray(donut.transform, pile);
            }
        }

        public void PickUpTrash(Transform child)
        {
            Stock.AddTrash(child.gameObject);
            _trayController.SetCharacter(this).PlayAddToTray(child);
        }

        public void DropTrash(Transform trashDropPos)
        {
            var trash = Stock.RemoveTrash();
            _trayController.PlayPutDownFromTray(trash.transform, trashDropPos);
        }

        #endregion
    }
}

