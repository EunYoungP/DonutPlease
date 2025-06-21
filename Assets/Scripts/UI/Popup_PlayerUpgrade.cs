using DonutPlease.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_PlayerUpgrade : UIPopup
{
    private readonly List<PageData> _pageDataList = new List<PageData>
    {
        new PageData { dataFieldName = "capacityGrade", title = "�뷮", imageName = "Player_capacity", needGem = 3, needCash = 50 },
        new PageData { dataFieldName = "moveSpeedGrade", title = "�̵��ӵ�", imageName = "Player_moveSpeed", needGem = 3, needCash = 100 },
        new PageData { dataFieldName = "profitGrowthGrade", title = "���ͻ��", imageName = "Player_ProfitGrowth", needGem = 3, needCash = 100 },
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
        var playerData = GameManager.GetGameManager.Data.SaveData.playerData;

        for (int i = 0; i < _pageDataList.Count; i++)
        {
            var pageData = _pageDataList[i];
            string pageId = pageData.dataFieldName;

            foreach (var data in playerData.GetType().GetFields())
            {
                if (data.Name == pageId)
                {
                    pageData.upgradeLevel = (int)data.GetValue(playerData);
                    break;
                }
            }

            _pages[i].SetPage(pageData, PageType.Player);
        }
    }
}
