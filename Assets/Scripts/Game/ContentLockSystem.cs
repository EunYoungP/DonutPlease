using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;

public static class ContentLockSystem
{
    public static void Initialize()
    {
        // ���� ���� ��ȭ �̺�Ʈ�� ���

        GameManager.GetGameManager.Player.Growth.Level.Subscribe(level =>
        {
            if (level == 2)
            {
                TryUnlock("HR");
            }
            else if (level == 3)
            {
                TryUnlock("Upgrade");
            }
            else if (level == 4)
            {
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
        { "HR", () => GameManager.GetGameManager.Player.Growth.Level.Value>= 2 },
        { "Upgrade", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 3 },
        { "DriveThru", () => GameManager.GetGameManager.Player.Growth.Level.Value >= 4 },
    };

    public static List<string> Contents => contentUnlockCallbacks.Keys.ToList();

    #region callback unlcok

    private static void UnlockHRCallback()
    {
        // HR UIIntercation ����
        GameManager.GetGameManager.Intercation.CreateInteractionUI(100);

        // ���� ����
        GameManager.GetGameManager.Store.CreateWorker();

        // ������ ���� �˾� ���
    }

    private static void UnlockUpgradeCallback()
    {
        // Upgrade UIInteraction ����
        GameManager.GetGameManager.Intercation.CreateInteractionUI(200);

        // ������ ���� �˾� ���
    }

    private static void UnlockDriveThruCallback()
    {
        // DriveThru UIInteraction ����
        GameManager.GetGameManager.Intercation.CreateInteractionUI(300);

        // ������ ���� �˾� ���
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
