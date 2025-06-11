using System.IO;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public WorkerData workerData;
    public StageData stageData;
    public ContentLockData[] contentLocks;
}

[System.Serializable]
public class PlayerData
{
    public int level;
    public int exp;
    public int moveSpeed;
    public int capacity;
    public int cash;
    public int gem;
}

[System.Serializable]
public class StageData
{
    public int stageLevel;
}

[System.Serializable]
public class WorkerData
{
    public int capacity;
    public int moveSpeed;
}

[System.Serializable]
public class ContentLockData
{
    public string contentId;
    public bool isUnlocked;
}

public static class DataManager
{
    static string saveFileDir = Application.dataPath;

    private static readonly string SavePath;
    public static SaveData SaveData { get; private set; }

    static DataManager()
    {
#if UNITY_EDITOR
        saveFileDir = Directory.GetParent(Application.dataPath).FullName;
#else
    saveFileDir = Application.persistentDataPath;
#endif
        SavePath = Path.Combine(saveFileDir, "save.json");
    }

    private static SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            playerData = new PlayerData { level = 1, exp = 0, moveSpeed = 5, capacity = 10, cash = 300, gem = 0 },
            workerData = new WorkerData { capacity = 5, moveSpeed = 3},
            stageData = new StageData { stageLevel = 1},
            contentLocks = new ContentLockData[]
            {
                new ContentLockData { contentId = "worker", isUnlocked = false },
                new ContentLockData { contentId = "stage", isUnlocked = false },
                new ContentLockData { contentId = "shop", isUnlocked = false },
                new ContentLockData { contentId = "settings", isUnlocked = false }
            }
        };
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void Load(out SaveData data)
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
}
