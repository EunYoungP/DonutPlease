using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;

public static class ContentLockSystem
{
    public static void Initialize()
    {
        // 게임 상태 변화 이벤트들 등록

        GameManager.GetGameManager.Player.Growth.Level.Subscribe(level =>
        {
            if (level == 2)
            {
                TryUnlock("HRInteraction");
            }
            else if (level == 4)
            {
                TryUnlock("UpgradeInteraction");
            }
            else if (level == 6)
            {
                TryUnlock("DriveThruInteraction");
            }
        });

        FluxSystem.ActionStream.Subscribe(data =>
        {
            if (data is FxOnCompleteUIInteraction fxOnCompleteUIInteraction)
            {
                if (fxOnCompleteUIInteraction.interactionId == 100) // HR Interaction
                    TryUnlock("HR");
                else if (fxOnCompleteUIInteraction.interactionId == 200)
                    TryUnlock("Upgrade");
                else if (fxOnCompleteUIInteraction.interactionId == 300)
                    TryUnlock("DriveThru");
            }
        });

        TryUnlock("HRInteraction");
        TryUnlock("UpgradeInteraction");
        TryUnlock("DriveThruInteraction");
        TryUnlock("HR");
        TryUnlock("Upgrade");
        TryUnlock("DriveThru");
    }

    private static Dictionary<string, Action> contentUnlockCallbacks = new Dictionary<string, Action>()
    {
        { "HRInteraction", UnlockHRInteractionCallback },
        { "HR", UnlockHRCallback },
        { "UpgradeInteraction", UnlockUpgradeInteractionCallback },
        { "Upgrade", UnlockUpgradeCallback },
        { "DriveThruInteraction", UnlockDriveThruInteractionCallback },
        { "DriveThru", UnlockDriveThruCallback },
    };

    private static Dictionary<string, Func<bool>> conditions = new Dictionary<string, Func<bool>>()
    {
        { "HRInteraction", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 2 },
        { "HR", () => GameManager.GetGameManager.Intercation.IsCompleteUIInteractionDataInStore(100)},
        { "UpgradeInteraction", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 4 },
        { "Upgrade", () => GameManager.GetGameManager.Intercation.IsCompleteUIInteractionDataInStore(200) },
        { "DriveThruInteraction", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 6 },
        { "DriveThru", () => GameManager.GetGameManager.Intercation.IsCompleteUIInteractionDataInStore(300)},
    };

    public static List<string> Contents => contentUnlockCallbacks.Keys.ToList();

    #region callback unlcok

    private static void UnlockHRInteractionCallback()
    {
        GameManager.GetGameManager.Intercation.CreateInteractionUI(100);
    }

    private static void UnlockHRCallback()
    {
        var store = GameManager.GetGameManager.Store.GetStore(1);
        for(int i = 0; i < store.Stat.HiredCount.Value; i++)
            GameManager.GetGameManager.Store.CreateWorker(1);

        GameManager.GetGameManager.Tutorial.StartTutorial(1);
    }

    private static void UnlockUpgradeInteractionCallback()
    {
        GameManager.GetGameManager.Intercation.CreateInteractionUI(200);
    }
    
    private static void UnlockUpgradeCallback()
    {
        // Upgrade UIInteraction 생성
        GameManager.GetGameManager.Tutorial.StartTutorial(2);
    }

    private static void UnlockDriveThruInteractionCallback()
    {
        GameManager.GetGameManager.Intercation.CreateInteractionUI(300);
    }

    private static void UnlockDriveThruCallback()
    {
        // DriveThru UIInteraction 생성
        GameManager.GetGameManager.Tutorial.StartTutorial(3);
    }

    #endregion

    public static bool TryUnlock(string contentId)
    {
        if (IsUnlocked(contentId))
            return true;

        if (conditions.TryGetValue(contentId, out var condition))
        {
            if (condition())
            {
                SetUnlock(contentId);
                return true;
            }
        }
        return false;
    }

    public static bool IsUnlocked(string contentId)
    {
        foreach( var content in GameManager.GetGameManager.Data.SaveData.contentLocks)
        {
            if (content.contentId == contentId && content.isUnlocked)
            {
                return true;
            }
        }
        return false;
    }

    private static void SetUnlock(string contentId)
    {
        GameManager.GetGameManager.Data.UpgradeContentLockData(contentId, true);

        if (contentUnlockCallbacks.TryGetValue(contentId, out var callback))
            callback.Invoke();
    }
}
