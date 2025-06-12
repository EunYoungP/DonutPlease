using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DonutPlease.UI
{
    public struct PageData
    {
        public string pageId;
        public string title;
        public string imageName;
        public int upgradeLevel;
        public int needGen;
        public int needCash;
    }

    public class Popup_HR : UIPopup
    {
        private readonly List<PageData> _pageDataList = new List<PageData>
        {
            new PageData { pageId = "capacity", title = "Capacity Upgrade", imageName = "HR_capacity", needGen = 3, needCash = 50 },
            new PageData { pageId = "moveSpeed", title = "MoveSpeed Upgrade", imageName = "HR_moveSpeed", needGen = 3, needCash = 100 },
            new PageData { pageId = "hiredCount", title = "HiredCount Upgrade", imageName = "HR_hiredCount", needGen = 3, needCash = 100 },
        };

        [SerializeField] private Button _closeBtn;
        [SerializeField] private List<UI_HRPage> _pages;

        protected override void Awake()
        {
            _closeBtn.onClick.AddListener(() => Hide());

            Initialize();
        }

        public void Initialize()
        {
            var hrData = GameManager.GetGameManager.Data.SaveData.hrData;

            for(int i = 0; i < _pageDataList.Count; i++)
            {
                var pageData = _pageDataList[i];
                string pageId = pageData.pageId;

                foreach (var data in hrData.GetType().GetFields())
                {
                    if (data.Name == pageId)
                    {
                        pageData.upgradeLevel = (int)data.GetValue(hrData);
                        break;
                    }
                }

                _pages[i].SetPage(pageData);
            }
        }
    }
}
