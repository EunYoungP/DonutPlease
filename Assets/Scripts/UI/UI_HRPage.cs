using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DonutPlease.UI
{
    public enum PageType
    {
        HR,
        Player
    }

    public class UI_HRPage : UIBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Image _image;
        [SerializeField] private List<Image> _upgradeLevelIcons;
        [SerializeField] private Button_ImageText _upgradeByGemBtn;
        [SerializeField] private Button_ImageText _upgradeByCashBtn;

        private PageType _pageType;
        private PageData _pageData;

        protected override void Awake()
        {
            _upgradeByGemBtn.SetCallback(() => OnUpgradeByGem());
            _upgradeByCashBtn.SetCallback(() => OnUpgradeByCash());
        }

        public void SetPage(PageData pageData, PageType pageType)
        {
            _pageType = pageType;

            var gameMng = GameManager.GetGameManager;

            _pageData = pageData;
            _titleText.text = pageData.title;
            _image.sprite = gameMng.Resource.GetAsset(gameMng.Resource.GetUIResourcePath(pageData.imageName)).GetComponent<Image>().sprite;
            for (int i = 0; i < pageData.upgradeLevel; i++)
            {
                _upgradeLevelIcons[i].gameObject.SetActive(true);
            }

            _upgradeByGemBtn.SetText(pageData.needGen.ToString());
            _upgradeByGemBtn.SetImage(pageData.needCash.ToString());
        }

        private void UpdateUpgradeLevel()
        {
            for (int i = 0; i < _pageData.upgradeLevel; i++)
            {
                _upgradeLevelIcons[i].gameObject.SetActive(true);
            }
        }

        #region OnButtonEvent

        private void OnUpgradeByGem()
        {
        }

        private void OnUpgradeByCash()
        {
            if (GameManager.GetGameManager.Player.Currency.PayCash(_pageData.needCash))
            {
                if (_pageType == PageType.HR)
                {
                    GameManager.GetGameManager.Store.CurStore.UpgradeWorkerData(_pageData.dataFieldName, 1);
                }
                else if (_pageType == PageType.Player)
                {
                    GameManager.GetGameManager.Player.Character.Stat.UpgradePlayerData(_pageData.dataFieldName, 1);
                }
            }

            UpdateUpgradeLevel();
        }

        #endregion
    }
}
