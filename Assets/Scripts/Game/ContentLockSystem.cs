using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ContentLockSystem
{
    static ContentLockSystem()
    {
        // ∞‘¿” ªÛ≈¬ ∫Ø»≠ ¿Ã∫•∆ÆµÈ µÓ∑œ
    }

    private static Dictionary<string, Action> contents = new Dictionary<string, Action>()
    {
        { "HR", UnlockHRCallback },
        { "Upgrade", UnlockUpgradeCallback },
        { "DriveThru", UnlockDriveThruCallback },
    };

    private static Dictionary<string, Func<bool>> conditions = new Dictionary<string, Func<bool>>()
    {
        { "HR", () => DataManager.SaveData.playerData.level>= 2 },
        { "Upgrade", () => DataManager.SaveData.playerData.level >= 3 },
        { "DriveThru", () => DataManager.SaveData.playerData.level >= 4 },
    };

    #region callback unlcok
    private static void UnlockHRCallback()
    {
        // HR UIIntercation ª˝º∫
        GameManager.GetGameManager.Intercation.CreateInteractionUI(100);

        // ƒ¡≈Ÿ√˜ ø¿«¬ ∆Àæ˜ √‚∑¬
    }

    private static void UnlockUpgradeCallback()
    {
        // Upgrade UIInteraction ª˝º∫
        GameManager.GetGameManager.Intercation.CreateInteractionUI(200);

        // ƒ¡≈Ÿ√˜ ø¿«¬ ∆Àæ˜ √‚∑¬
    }

    private static void UnlockDriveThruCallback()
    {
        // DriveThru UIInteraction ª˝º∫
        GameManager.GetGameManager.Intercation.CreateInteractionUI(300);

        // ƒ¡≈Ÿ√˜ ø¿«¬ ∆Àæ˜ √‚∑¬
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
        return false;
    }

    private static void SetUnlock(string contentId)
    {
        var list = DataManager.SaveData.contentLocks.ToList();
        ContentLockData contentLockData = list.Find(c => c.contentId == contentId);
        if (contentLockData != null)
            contentLockData.isUnlocked = true;
        else
            list.Add(new ContentLockData { contentId = contentId, isUnlocked = true });

        // ¿˙¿Â « ø‰
        DataManager.Save(); 
    }
}
