using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData 
{
    //public static bool isStartGame = true;

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

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

}
