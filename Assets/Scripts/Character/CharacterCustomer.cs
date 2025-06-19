using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterCustomer : CharacterBase
    {
        [SerializeField] private CharacterCustomerController _controller;
        [SerializeField] private TrayController _trayController;

        public CharacterCustomerController Controller => _controller;

        public CharacterStockComponent Stock { get; private set; }

        private void Awake()
        {
            Stock = new CharacterStockComponent();

            _uiFollowCharacter.Initialize(this);
        }

        public void Pay(int getDonutCount, CashPile cashPile)
        {
            cashPile.MakeCashInPile(this, getDonutCount);
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

        #endregion
    }
}

