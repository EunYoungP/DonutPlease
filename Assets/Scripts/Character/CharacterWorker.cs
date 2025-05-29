using DonutPlease.Game.Character;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

        protected override void AddToTray(CharacterBase character, Transform child)
        {
            if (character is CharacterWorker worker)
            {
                Stock.AddDonut(child.gameObject);
                _trayController.SetCharacter(worker).PlayAddToTray(child);
            }
        }

        protected override void RemoveFromTray(CharacterBase character, PileBase pile)
        {
            if (character is CharacterWorker worker)
            {
                var donut = Stock.RemoveDonut();
                _trayController.PlayPutDownFromTray(donut.transform, pile);
            }
        }

        #endregion

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
    }
}

