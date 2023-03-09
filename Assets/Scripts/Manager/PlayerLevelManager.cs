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

    private void Start()
    {
        instance = this;
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnEnemyDead, (param) => OnEnemyDead((int)param));
    }

    public void OnEnemyDead(int exp)
    {
        currentExp += exp;
        if (currentExp >= maxExp)
        {
            currentExp = currentExp - maxExp;
            currentLevel++;
            maxExp = maxExp * 1.5f;
        }
    }
}
