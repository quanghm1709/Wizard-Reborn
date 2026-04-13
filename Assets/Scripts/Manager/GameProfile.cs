using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameProfile
{
    public bool isStartGame = true;
    public int floor = 1;
    public int specialRoom = 1;
    
    // Player Stats
    public int playerCurrentHp;
    public int playerCurrentMp;
    public int playerMaxHp;
    public int playerMaxMp;
    public int playerCurrentAtk;
    public int playerMaxAtk;

    // Player Level
    public int level = 1;
    public int exp = 0;
    public int skillPoint = 0;

    // Skills
    public List<SkillSaveData> skills = new List<SkillSaveData>();

    // Active Skills bindings
    public List<ActiveSkillBinding> activeSkills = new List<ActiveSkillBinding>();
}

[Serializable]
public class SkillSaveData
{
    public int skillId;
    public int treePos;
    public int skillLevel;
}

[Serializable]
public class ActiveSkillBinding
{
    public int pos;
    public int treeIndex;
}
