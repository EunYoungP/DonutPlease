using DonutPlease.Game.Character;
using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterWorker : CharacterBase
    {
        [SerializeField] private CharacterWorkerController _controller;
        [SerializeField] private TrayController _trayController;

        public CharacterWorkerController Controller => _controller;

        #region Tray

        protected override void AddToTray(Transform child)
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

