using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGameData
{
    public int version = SaveData.CurrentVersion;
    public int floor = 1;
    public int specialRoom = 1;
    public int gold;
    public PlayerSaveData player = new PlayerSaveData();
    public PlayerLevelSaveData progression = new PlayerLevelSaveData();
    public List<SkillTreeSaveData> skillTrees = new List<SkillTreeSaveData>();
    public List<EquippedSkillSaveData> equippedSkills = new List<EquippedSkillSaveData>();
}

[System.Serializable]
public class PlayerSaveData
{
    public int currentHp;
    public int maxHp;
    public float currentMp;
    public float maxMp;
    public int currentAtk;
    public int maxAtk;
    public float currentSpd;
    public float maxSpd;
    public float attackCooldown;
}

[System.Serializable]
public class PlayerLevelSaveData
{
    public int level = 1;
    public float currentExp;
    public float maxExp;
    public int skillPoint;
}

[System.Serializable]
public class SkillTreeSaveData
{
    public int treePosition;
    public string treeType;
    public List<int> skillLevels = new List<int>();
}

[System.Serializable]
public class EquippedSkillSaveData
{
    public int slot;
    public string skillId;
}

public static class SaveData 
{
    public const int CurrentVersion = 2;
    private const string GameSaveKey = "WizardReborn.Save.V2";

    public static void SaveGame(SaveGameData data)
    {
        if (data == null)
        {
            return;
        }

        data.version = CurrentVersion;
        PlayerPrefs.SetString(GameSaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static bool TryLoadGame(out SaveGameData data)
    {
        data = null;
        if (!PlayerPrefs.HasKey(GameSaveKey))
        {
            return false;
        }

        try
        {
            data = JsonUtility.FromJson<SaveGameData>(PlayerPrefs.GetString(GameSaveKey));
            return data != null && data.version > 0 && data.version <= CurrentVersion;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"Cannot load versioned save: {exception.Message}");
            data = null;
            return false;
        }
    }

    public static void SavePlayerData(string key, List<int> value)
    {
        PlayerPrefs.SetInt(key + "count", value.Count);
        for (int i = 0; i < value.Count; i++)
        {
            PlayerPrefs.SetInt(key + i, value[i]);
        }
    }
    public static void SaveSingleData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static List<int> LoadPlayerData(string key)
    {
        List<int> data = new List<int>();
        int count = PlayerPrefs.GetInt(key + "count");

        for (int i = 0; i < count; i++)
        {
            data.Add(PlayerPrefs.GetInt(key + i));
        }

        return data;
    }

    public static int LoadSingleData(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static bool HasKey(string key)
    {
        if (PlayerPrefs.HasKey(key + "count"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool HasSingleKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

}
