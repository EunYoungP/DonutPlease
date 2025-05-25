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
        }

        #region Tray

        protected override void AddToTray(CharacterBase customer, Transform child)
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

        #endregion
    }
}

