using DG.Tweening;
using System;
using UnityEngine;

using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;

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

            if (item.ItemType == EItemType.Cash)
                AddCashToCharacter(item);

            if (Stock.CanGetItemType(item.ItemType))
            {
                // 캐쉬는 Currency에 저장
                Stock.AddItem(item);
                _trayController.PlayAddToTray(item);
            }
        }

        private void AddCashToCharacter(Item cashItem)
        {
            if (cashItem == null)
            {
                Debug.LogError("Item component is missing on the child object.");
                return;
            }

            Vector3 dest = transform.position + Vector3.up;

            cashItem.transform.DOJump(dest, 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    Destroy(cashItem.gameObject);
                });
        }

        public void RemoveCashFromPlayerTo(Transform dest, Action callback = null)
        {
            GameObject prefab = GameManager.GetGameManager.Resource.GetCash();
            GameObject cash = Instantiate(prefab, transform.position, Quaternion.identity);

            // 여기로 이동
            cash.transform.DOJump(gameObject.transform.position, 5, 1, 0.1f)
                .OnComplete(() =>
                {
                    callback?.Invoke();

                    Destroy(cash);
                });
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

