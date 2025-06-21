using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterWorker : CharacterBase
    {
        [SerializeField] private CharacterWorkerController _controller;
        [SerializeField] private TrayController _trayController;

        public CharacterWorkerController Controller => _controller;

        public CharacterStockComponent Stock { get; private set; }

        private void Awake()
        {
            Stock = new CharacterStockComponent();

            _uiFollowCharacter.Initialize(this);
        }

        #region Tray

        public override void AddToTray(Transform child)
        {
            var item = child.GetComponent<Item>();

            if (Stock.CanGetItemType(item.ItemType))
            {
                Stock.AddItem(item);
                _trayController.SetCharacter(this).PlayAddToTray(item);
            }
        }

        public override void RemoveFromTray(EItemType itemType, PileBase pile)
        {
            var item = Stock.RemoveItem(itemType);
            if (item == null)
                return;

            _trayController.PlayPutDownFromTray(item.transform, pile);
        }

        public override void RemoveFromTray(EItemType itemType, Transform targetPos)
        {
            var item = Stock.RemoveItem(itemType);
            if (item == null)
                return;

            _trayController.PlayPutDownFromTray(item.transform, targetPos);
        }

        #endregion
    }
}

