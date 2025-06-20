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
            else if (level == 3)
            {
                TryUnlock("UpgradeInteraction");
            }
            else if (level == 4)
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
        { "HR", () => GameManager.GetGameManager.Intercation.GetUIInteractionDataInStore(100).isComplete},
        { "UpgradeInteraction", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 3 },
        { "Upgrade", () => GameManager.GetGameManager.Intercation.GetUIInteractionDataInStore(200).isComplete },
        { "DriveThruInteraction", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 4 },
        { "DriveThru", () => GameManager.GetGameManager.Intercation.GetUIInteractionDataInStore(300).isComplete},
    };

    public static List<string> Contents => contentUnlockCallbacks.Keys.ToList();

    #region callback unlcok

    private static void UnlockHRInteractionCallback()
    {
        GameManager.GetGameManager.Intercation.CreateInteractionUI(100);
    }

    private static void UnlockHRCallback()
    {
        GameManager.GetGameManager.Store.CreateWorker();

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
