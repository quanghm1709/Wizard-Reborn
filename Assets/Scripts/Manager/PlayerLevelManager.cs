using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager instance;

    [SerializeField] private int currentLevel;
    [SerializeField] private float currentExp;
    [SerializeField] private float maxExp;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIController.instance.GetPlayerCurrentLevel(currentLevel, currentExp, maxExp);
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnEnemyDead, HandleEnemyDead);
        this.RegisterListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.OnEnemyDead, HandleEnemyDead);
        this.RemoveListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void HandleEnemyDead(object param)
    {
        if (param is int exp)
        {
            OnEnemyDead(exp);
        }
    }

    private void HandlePlayerEnterGate(object param)
    {
        OnPlayerEnterGate();
    }

    private void OnPlayerEnterGate()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().Save();

        SaveData.SaveSingleData("level", currentLevel);
        SaveData.SaveSingleData("exp", (int) currentExp);
        SaveData.SaveSingleData("skillPoint", 0);
    }

    public void OnEnemyDead(int exp)
    {
        currentExp += exp;
        while (currentExp >= maxExp && maxExp > 0f)
        {
            currentExp = currentExp - maxExp;
            currentLevel++;
            maxExp = maxExp * 1.5f;
            GameObject.Find("Player").GetComponent<PlayerController>().LevelUp();
            this.PostEvent(EventID.OnPlayerLevelUp, currentLevel);
        }
        UIController.instance.GetPlayerCurrentLevel(currentLevel, currentExp, maxExp);
    }

    internal void Load()
    {
        currentLevel = SaveData.LoadSingleData("level");
        currentExp = SaveData.LoadSingleData("exp");

        if (currentLevel > 1)
        {
            maxExp *= Mathf.Pow(1.5f, currentLevel - 1);
        }
    }

    public PlayerLevelSaveData CaptureSaveData()
    {
        return new PlayerLevelSaveData
        {
            level = currentLevel,
            currentExp = currentExp,
            maxExp = maxExp,
            skillPoint = 0
        };
    }

    public void ApplySaveData(PlayerLevelSaveData data)
    {
        if (data == null)
        {
            return;
        }

        currentLevel = Mathf.Max(1, data.level);
        maxExp = data.maxExp > 0f ? data.maxExp : Mathf.Max(1f, maxExp);
        currentExp = Mathf.Clamp(data.currentExp, 0f, maxExp);
        if (UIController.instance != null)
        {
            UIController.instance.GetPlayerCurrentLevel(currentLevel, currentExp, maxExp);
        }
    }
}
