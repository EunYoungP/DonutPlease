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

        public override void AddToTray(Transform child)
        {
            var item = child.GetComponent<Item>();

            if (Stock.CanGetItemType(item.ItemType))
            {
                Stock.AddItem(item);
                _trayController.PlayAddToTray(item);
            }
        }

        public override void RemoveFromTray(EItemType itemType, PileBase pile)
        {
            var item = Stock.RemoveItem(itemType);
            _trayController.PlayPutDownFromTray(item.transform, pile);
        }

        public override void RemoveFromTray(EItemType itemType, Transform trashDropPos)
        {
            var item = Stock.RemoveItem(itemType);
            if (item == null)
                return;

            _trayController.PlayPutDownFromTray(item.transform, trashDropPos);
        }
        #endregion
    }
}

