using DG.Tweening;
using System;
using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterPlayer : CharacterBase
    {
        [SerializeField] private CharacterPlayerController _controller;
        [SerializeField] private TrayController _trayController;

        private PlayerCamera _camera;

        public CharacterStockComponent Stock { get; private set; }
        public PlayerStatComponent Stat { get; private set; }

        public CharacterPlayerController Controller => _controller;

        public void Initialize()
        {
            Stock = new CharacterStockComponent();
            Stat = new PlayerStatComponent();

            _camera = Camera.main.GetComponent<PlayerCamera>();
            _camera.Initialize(this);

            _controller.Initialize();
        }


        #region Tray

        public override void AddToTray(Transform child)
        {
            var item = child.GetComponent<Item>();
            if (item.ItemType == EItemType.Cash)
                GetCash(item);

            if (Stock.CanGetItemType(item.ItemType))
            {
                Stock.AddItem(item);
                _trayController.PlayAddToTray(item);
                
            }
        }

        private void GetCash(Item cashItem)
        {
            if (cashItem == null)
            {
                Debug.LogError("Item component is missing on the child object.");
                return;
            }

            FluxSystem.Dispatch(new FxOnUpdateCurrency(CurrencyType.Cash, cashItem.RewardCash));

            Vector3 dest = transform.position + Vector3.up;
            cashItem.transform.DOJump(dest, 5, 1, 0.3f)
                .OnComplete(() =>
                {
                    Destroy(cashItem.gameObject);
                });
        }

        public override void RemoveFromTray(EItemType itemType, PileBase pile)
        {
            var item = Stock.RemoveItem(itemType);
            if (item == null)
                return;

            _trayController.PlayPutDownFromTray(item.transform, pile);
        }

        public override void RemoveFromTray(EItemType itemType, Transform trashDropPos)
        {
            var item = Stock.RemoveItem(itemType);
            if (item == null)
                return;

            _trayController.PlayPutDownFromTray(item.transform, trashDropPos);
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

        #endregion
    }
}

