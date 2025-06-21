using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DonutPlease.UI
{
    public struct PageData
    {
        public string dataFieldName;
        public string title;
        public string imageName;
        public int upgradeLevel;
        public int needGem;
        public int needCash;
    }

    public class Popup_HR : UIPopup
    {
        private readonly List<PageData> _pageDataList = new List<PageData>
        {
            new PageData { dataFieldName = "capacityGrade", title = "용량", imageName = "HR_capacity", needGem = 3, needCash = 50 },
            new PageData { dataFieldName = "moveSpeedGrade", title = "이동속도", imageName = "HR_moveSpeed", needGem = 3, needCash = 100 },
            new PageData { dataFieldName = "hiredCountGrade", title = "고용", imageName = "HR_hiredCount", needGem = 3, needCash = 100 },
        };

        [SerializeField] private Button _closeBtn;
        [SerializeField] private List<UI_Page> _pages;

        protected override void Awake()
        {
            _closeBtn.onClick.AddListener(() => Hide());

            Initialize();
        }

        public void Initialize()
        {
            var hrData = GameManager.GetGameManager.Data.SaveData.storeData.hrData;

            for(int i = 0; i < _pageDataList.Count; i++)
            {
                var pageData = _pageDataList[i];
                string pageId = pageData.dataFieldName;

                foreach (var data in hrData.GetType().GetFields())
                {
                    if (data.Name == pageId)
                    {
                        pageData.upgradeLevel = (int)data.GetValue(hrData);
                        break;
                    }
                }

                _pages[i].SetPage(pageData, PageType.HR);
            }
        }
    }
}
