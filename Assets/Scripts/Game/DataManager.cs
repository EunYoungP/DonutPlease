using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public HRData hrData;
    public StoreData stageData;
    public List<ContentLockData> contentLocks;
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
public class StoreData
{
    public int storeID;
}

[System.Serializable]
public class HRData
{
    public int capacity;
    public int moveSpeed;
    public int hiredCount;
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

    private void Awake()
    {
#if UNITY_EDITOR
        saveFileDir = Directory.GetParent(Application.dataPath).FullName;
#else
    saveFileDir = Application.persistentDataPath;
#endif
        SavePath = Path.Combine(saveFileDir, "save.json");
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
            playerData = new PlayerData { level = 1, exp = 0, moveSpeed = 1, capacity = 10, cash = 1000, gem = 0 },
            hrData = new HRData { capacity = 1, moveSpeed = 1, hiredCount = 1 },
            stageData = new StoreData { storeID = 1 },
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
}
