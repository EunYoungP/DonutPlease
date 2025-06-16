using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UniRx;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public StoreData storeData;
    public List<ContentLockData> contentLocks;
}

[System.Serializable]
public class PlayerData
{
    public int level;
    public int exp;
    public int moveSpeedGrade;
    public int capacityGrade;
    public int profitGrowthGrade;
    public int cash;
    public int gem;
}

[System.Serializable]
public class StoreData
{
    public int storeID;
    public HRData hrData;
}

[System.Serializable]
public class HRData
{
    public int capacityGrade;
    public int moveSpeedGrade;
    public int hiredCountGrade;
}

[System.Serializable]
public class ContentLockData
{
    public string contentId;
    public bool isUnlocked;
}

public class DataManager : MonoBehaviour
{
    static string saveFileDir = Application.dataPath;

    private static string SavePath;
    public SaveData SaveData { get; private set; }

    public DataManager Initialize()
    {
#if UNITY_EDITOR
        saveFileDir = Directory.GetParent(Application.dataPath).FullName;
#else
    saveFileDir = Application.persistentDataPath;
#endif
        SavePath = Path.Combine(saveFileDir, "save.json");

        return this;
    }

    private SaveData CreateDefaultSaveData()
    {
        List<ContentLockData> contentLocks = new();
        foreach (var content in ContentLockSystem.Contents)
        {
            contentLocks.Add(new ContentLockData { contentId = content, isUnlocked = false });
        }

        return new SaveData
        {
            playerData = new PlayerData { level = 1, exp = 0, moveSpeedGrade = 0, capacityGrade = 0, profitGrowthGrade = 0,  cash = 1000, gem = 0 },
            storeData = new StoreData { storeID = 1, hrData = new HRData { capacityGrade = 0, moveSpeedGrade = 0, hiredCountGrade = 0 } },
            contentLocks = contentLocks
        };
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(SavePath, json);
    }

    public void Load(out SaveData data)
    {
        if (!File.Exists(SavePath))
        {
            SaveData = CreateDefaultSaveData();
            Save();
        }
        else
        {
            string json = File.ReadAllText(SavePath);
            SaveData = JsonUtility.FromJson<SaveData>(json);
        }

        data = SaveData;
    }

    public void SavePlayerData(int moveSpeedGrade, int donutCapacityGrade, int profitGrowthRateGrade)
    {
        SaveData.playerData.moveSpeedGrade = moveSpeedGrade;
        SaveData.playerData.capacityGrade = donutCapacityGrade;
        SaveData.playerData.profitGrowthGrade = profitGrowthRateGrade;

        Save();
    }

    public void SaveHRData(int moveSpeedGrade, int donutCapacityGrade, int hiredCountGrade)
    {
        SaveData.storeData.hrData.moveSpeedGrade = moveSpeedGrade;
        SaveData.storeData.hrData.capacityGrade = donutCapacityGrade;
        SaveData.storeData.hrData.hiredCountGrade = hiredCountGrade;

        Save();
    }

    public void UpgradePlayerGrowth(int level, int exp)
    {
        SaveData.playerData.level = level;
        SaveData.playerData.exp = exp;

        Save();
    }

    public void UpgradePlayerCurrency(CurrencyType type, int value)
    {
        if (type == CurrencyType.Cash)
        {
            SaveData.playerData.cash = value;
        }
        else if (type == CurrencyType.Gem)
        {
            SaveData.playerData.gem = value;
        }
        else
        {
            Debug.LogWarning($"[DataManager] Unknown currency type: {type}");
            return;
        }

        Save();
    }

    public void UpgradeContentLockData(string contentId, bool isUnlocked)
    {
        ContentLockData contentLockData = SaveData.contentLocks.Find(c => c.contentId == contentId);
        if (contentLockData != null)
            contentLockData.isUnlocked = true;
        else
            SaveData.contentLocks.Add(new ContentLockData { contentId = contentId, isUnlocked = true });

        Save();
    }
}
