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
    [SerializeField] private int skillPoint;

    private void Start()
    {
        instance = this;
        UIController.instance.GetPlayerCurrentLevel(currentLevel, currentExp, maxExp);
        RegisterEvent();
    }

    private void Update()
    {
        SkillUIManager.instance.skillPoint.text = "Skill Point: " + skillPoint.ToString();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnEnemyDead, (param) => OnEnemyDead((int)param));
        this.RegisterListener(EventID.OnSkillUpgradeClick, (param) => OnSkillUpgradeClick());
        this.RegisterListener(EventID.OnSkillUpgradeFailed, (param) => OnSkillUpgradeFailed());
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => OnPlayerEnterGate());
    }

    private void OnPlayerEnterGate()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().Save();

        SaveData.SaveSingleData("level", currentLevel);
        SaveData.SaveSingleData("exp", (int) currentExp);
        SaveData.SaveSingleData("skillPoint", skillPoint);
    }

    private void OnSkillUpgradeFailed()
    {
        skillPoint++;
    }

    private void OnSkillUpgradeClick()
    {
        if (skillPoint > 0)
        {
            skillPoint--;
            this.PostEvent(EventID.OnSkillUpgrade);
        }
    }

    public void OnEnemyDead(int exp)
    {
        currentExp += exp;
        if (currentExp >= maxExp)
        {
            currentExp = currentExp - maxExp;
            currentLevel++;
            skillPoint++;
            maxExp = maxExp * 1.5f;
            GameObject.Find("Player").GetComponent<PlayerController>().LevelUp();
        }
        UIController.instance.GetPlayerCurrentLevel(currentLevel, currentExp, maxExp);
    }

    internal void Load()
    {
        currentLevel = SaveData.LoadSingleData("level");
        currentExp = SaveData.LoadSingleData("exp");
        skillPoint = SaveData.LoadSingleData("skillPoint");

        if (currentLevel > 1)
        {
            maxExp = maxExp * 1.5f * (currentLevel-1);
        }
    }
}
