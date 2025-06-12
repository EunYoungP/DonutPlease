using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DonutPlease.UI
{
    public class UI_HRPage : UIBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Image _image;
        [SerializeField] private List<Image> _upgradeLevelIcons;
        [SerializeField] private Button_ImageText _upgradeByGemBtn;
        [SerializeField] private Button_ImageText _upgradeByCashBtn;

        private PageData _pageData;

        protected override void Awake()
        {
            _upgradeByGemBtn.SetCallback(() => OnClickPayGem());
            _upgradeByCashBtn.SetCallback(() => OnClickPayCash());
        }

        public void SetPage(PageData pageData)
        {
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

        private void OnClickPayGem()
        {

        }

        private void OnClickPayCash()
        {
            GameManager.GetGameManager.Player.Currency.PayCash(_pageData.needCash);
        }
    }
}
