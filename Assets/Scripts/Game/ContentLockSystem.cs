using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public static class ContentLockSystem
{
    public static void Initialize()
    {
        // °ÔÀÓ »óÅÂ º¯È­ ÀÌº¥Æ®µé µî·Ï

        FluxSystem.ActionStream
        .Subscribe(data =>
        {
            if (data is OnUpdatePlayerGrowth updateGrowth)
            {
                TryUnlock("HR");
                TryUnlock("Upgrade");
                TryUnlock("DriveThru");
            }
        });
    }

    private static Dictionary<string, Action> contentUnlockCallbacks = new Dictionary<string, Action>()
    {
        { "HR", UnlockHRCallback },
        { "Upgrade", UnlockUpgradeCallback },
        { "DriveThru", UnlockDriveThruCallback },
    };

    private static Dictionary<string, Func<bool>> conditions = new Dictionary<string, Func<bool>>()
    {
        { "HR", () => GameManager.GetGameManager.Player.Growth.Level>= 2 },
        { "Upgrade", () => GameManager.GetGameManager.Player.Growth.Level >= 3 },
        { "DriveThru", () => GameManager.GetGameManager.Player.Growth.Level >= 4 },
    };

    public static List<string> Contents => contentUnlockCallbacks.Keys.ToList();

    #region callback unlcok

    private static void UnlockHRCallback()
    {
        // HR UIIntercation »ý¼º
        GameManager.GetGameManager.Intercation.CreateInteractionUI(100);

        // ÄÁÅÙÃ÷ ¿ÀÇÂ ÆË¾÷ Ãâ·Â
    }

    private static void UnlockUpgradeCallback()
    {
        // Upgrade UIInteraction »ý¼º
        GameManager.GetGameManager.Intercation.CreateInteractionUI(200);

        // ÄÁÅÙÃ÷ ¿ÀÇÂ ÆË¾÷ Ãâ·Â
    }

    private static void UnlockDriveThruCallback()
    {
        // DriveThru UIInteraction »ý¼º
        GameManager.GetGameManager.Intercation.CreateInteractionUI(300);

        // ÄÁÅÙÃ÷ ¿ÀÇÂ ÆË¾÷ Ãâ·Â
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
