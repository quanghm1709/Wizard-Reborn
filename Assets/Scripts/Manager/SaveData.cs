using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveData 
{
    private static GameProfile _currentProfile;
    private static string SavePath => Application.dataPath + "/save.json";

    public static GameProfile CurrentProfile 
    {
        get 
        {
            if (_currentProfile == null) LoadProfile();
            return _currentProfile;
        }
    }

    public static void LoadProfile()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                _currentProfile = JsonUtility.FromJson<GameProfile>(json);
            }
            catch(System.Exception e)
            {
                Debug.LogError("Error loading save file: " + e.Message);
                _currentProfile = new GameProfile();
            }
        }
        else
        {
            _currentProfile = new GameProfile();
        }
    }

    public static void SaveProfile()
    {
        if (_currentProfile == null) _currentProfile = new GameProfile();
        string json = JsonUtility.ToJson(_currentProfile, true);
        File.WriteAllText(SavePath, json);
    }

    public static void SavePlayerData(string key, List<int> value)
    {
        CurrentProfile.playerCurrentHp = value[0];
        CurrentProfile.playerCurrentMp = value[1];
        CurrentProfile.playerMaxHp = value[2];
        CurrentProfile.playerMaxMp = value[3];
        CurrentProfile.playerCurrentAtk = value[4];
        CurrentProfile.playerMaxAtk = value[5];
        SaveProfile();
    }

    public static void SaveSingleData(string key, int value)
    {
        if (key.StartsWith("gskill"))
        {
            string[] parts = key.Substring(6).Split('|');
            int skillIndex = int.Parse(parts[0]);
            int treePos = int.Parse(parts[1]);

            var skill = CurrentProfile.skills.Find(s => s.skillId == skillIndex && s.treePos == treePos);
            if (skill == null)
            {
                skill = new SkillSaveData { skillId = skillIndex, treePos = treePos, skillLevel = value };
                CurrentProfile.skills.Add(skill);
            }
            else
            {
                skill.skillLevel = value;
            }
        }
        else if (key.StartsWith("pos"))
        {
            string[] parts = key.Substring(3).Split('|');
            int pos = int.Parse(parts[0]);
            int treeIndex = int.Parse(parts[1]);

            var act = CurrentProfile.activeSkills.Find(s => s.pos == pos && s.treeIndex == treeIndex);
            if (act == null)
            {
                CurrentProfile.activeSkills.Add(new ActiveSkillBinding { pos = pos, treeIndex = treeIndex });
            }
        }
        else if (key == "specialRoom") CurrentProfile.specialRoom = value;
        else if (key == "floor") CurrentProfile.floor = value;
        else if (key == "level") CurrentProfile.level = value;
        else if (key == "exp") CurrentProfile.exp = value;
        else if (key == "skillPoint") CurrentProfile.skillPoint = value;

        SaveProfile();
    }

    public static List<int> LoadPlayerData(string key)
    {
        return new List<int>
        {
            CurrentProfile.playerCurrentHp,
            CurrentProfile.playerCurrentMp,
            CurrentProfile.playerMaxHp,
            CurrentProfile.playerMaxMp,
            CurrentProfile.playerCurrentAtk,
            CurrentProfile.playerMaxAtk
        };
    }

    public static int LoadSingleData(string key)
    {
        if (key.StartsWith("gskill"))
        {
            string[] parts = key.Substring(6).Split('|');
            int skillIndex = int.Parse(parts[0]);
            int treePos = int.Parse(parts[1]);

            var skill = CurrentProfile.skills.Find(s => s.skillId == skillIndex && s.treePos == treePos);
            return skill != null ? skill.skillLevel : 0;
        }
        else if (key == "specialRoom") return CurrentProfile.specialRoom;
        else if (key == "floor") return CurrentProfile.floor;
        else if (key == "level") return CurrentProfile.level;
        else if (key == "exp") return CurrentProfile.exp;
        else if (key == "skillPoint") return CurrentProfile.skillPoint;

        return 0; 
    }

    public static bool HasKey(string key)
    {
        return File.Exists(SavePath);
    }

    public static bool HasSingleKey(string key)
    {
        if (key.StartsWith("pos"))
        {
            string[] parts = key.Substring(3).Split('|');
            int pos = int.Parse(parts[0]);
            int treeIndex = int.Parse(parts[1]);

            var act = CurrentProfile.activeSkills.Find(s => s.pos == pos && s.treeIndex == treeIndex);
            return act != null;
        }
        return false;
    }

    public static void ResetData()
    {
        _currentProfile = new GameProfile();
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}
