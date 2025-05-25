using DonutPlease.Game.Character;
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
        }

        #region Tray

        protected override void AddToTray(CharacterBase worker, Transform child)
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

