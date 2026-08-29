using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTreeType
{
    None,
    Electro,
    Fire,
    Ice,
    Wind,
}

[System.Serializable]
public class GSkillCore
{
    public SkillCore skillCore;
    public int skillLevel;
    public bool canUnlock;

    public int GetSkillLevel()
    {
        return 0;
    }

    public bool Action()
    {
        return skillCore != null && skillCore.Action(skillLevel);
    }
}

public sealed class SkillLevelUpCandidate
{
    public SkillTree tree;
    public GSkillCore skill;
    public SkillUI skillUI;
    public int index;
}

public class SkillTree : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private int treePos;
    [SerializeField] private SkillTreeType treeType;
    //[SerializeField] private List<SkillCore> listSkill;
    [SerializeField] private List<SkillUI> listSkillUI;

    [SerializeField] private List<GSkillCore> listSkill;
    private int currentSkill;

    private void Start()
    {
        RegisterEvent();
        RefreshUnlockStates();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void HandlePlayerEnterGate(object param)
    {
        OnPlayerEnterGate();
    }

    private void OnPlayerEnterGate()
    {
        for(int i = 0; i < listSkill.Count; i++)
        {
            SaveData.SaveSingleData("gskill" + i+"|"+treePos, listSkill[i].skillLevel);
        }
    }

    public bool ApplyLevelUpChoice(int skillIndex)
    {
        RefreshUnlockStates();
        if (skillIndex < 0 || skillIndex >= listSkill.Count)
        {
            return false;
        }

        GSkillCore selected = listSkill[skillIndex];
        if (!selected.canUnlock || selected.skillLevel >= 3 || selected.skillCore == null)
        {
            return false;
        }

        selected.skillLevel++;
        if (selected.skillCore.skillType == SkillCore.SkillType.Passive && selected.skillLevel == 1 && PassiveSkillHolder.instance != null)
        {
            PassiveSkillHolder.instance.AddPassiveSkill(selected, listSkillUI[skillIndex]);
        }

        if (selected.skillLevel >= 3 && skillIndex < listSkill.Count - 1)
        {
            listSkill[skillIndex + 1].canUnlock = true;
        }

        currentSkill = skillIndex;
        if (SkillUIManager.instance != null)
        {
            LoadUI(skillIndex);
        }
        return true;
    }

    public void GetLevelUpCandidates(List<SkillLevelUpCandidate> result)
    {
        if (result == null)
        {
            return;
        }

        RefreshUnlockStates();
        for (int i = 0; i < listSkill.Count; i++)
        {
            GSkillCore skill = listSkill[i];
            if (skill != null && skill.skillCore != null && skill.canUnlock && skill.skillLevel < 3)
            {
                result.Add(new SkillLevelUpCandidate
                {
                    tree = this,
                    skill = skill,
                    skillUI = i < listSkillUI.Count ? listSkillUI[i] : null,
                    index = i
                });
            }
        }
    }

    private void RefreshUnlockStates()
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            listSkill[i].canUnlock = i == 0 || listSkill[i].skillLevel > 0 ||
                                     (i > 0 && listSkill[i - 1].skillLevel >= 3);
        }
    }

    public void LoadSkill()
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            currentSkill = i;
            listSkill[i].skillLevel = SaveData.LoadSingleData("gskill" + i + "|" + treePos);        
            if (listSkill[i].skillLevel > 0)
            {
                listSkill[i].canUnlock = true;
                if (listSkill[i].skillCore.skillType == SkillCore.SkillType.Passive && PassiveSkillHolder.instance != null)
                {
                    PassiveSkillHolder.instance.AddPassiveSkill(listSkill[i], listSkillUI[i]);
                }
            }
            if (listSkill[i].skillLevel >= 3 && i < listSkill.Count - 1)
            {
                listSkill[i + 1].canUnlock = true;
            }
        }
        RefreshUnlockStates();
    }

    public SkillTreeSaveData CaptureSaveData()
    {
        SkillTreeSaveData data = new SkillTreeSaveData
        {
            treePosition = treePos,
            treeType = treeType.ToString()
        };

        foreach (GSkillCore skill in listSkill)
        {
            data.skillLevels.Add(skill != null ? skill.skillLevel : 0);
        }
        return data;
    }

    public void ApplySaveData(SkillTreeSaveData data)
    {
        if (data == null)
        {
            return;
        }
        for (int i = 0; i < listSkill.Count; i++)
        {
            GSkillCore skill = listSkill[i];
            int savedLevel = i < data.skillLevels.Count ? data.skillLevels[i] : 0;
            skill.skillLevel = Mathf.Clamp(savedLevel, 0, 3);
            skill.canUnlock = i == 0 || skill.skillLevel > 0 || (i > 0 && listSkill[i - 1].skillLevel >= 3);

            if (skill.skillLevel > 0 && skill.skillCore.skillType == SkillCore.SkillType.Passive && PassiveSkillHolder.instance != null)
            {
                PassiveSkillHolder.instance.AddPassiveSkill(skill, listSkillUI[i]);
            }
        }
        RefreshUnlockStates();
    }

    public bool TryGetSkill(string skillId, out GSkillCore skill, out SkillUI skillUI)
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            if (listSkill[i]?.skillCore != null && listSkill[i].skillCore.SkillId == skillId)
            {
                skill = listSkill[i];
                skillUI = listSkillUI[i];
                return true;
            }
        }

        skill = null;
        skillUI = null;
        return false;
    }

    public bool TryGetFirstUnlockedActive(out GSkillCore skill, out SkillUI skillUI)
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            if (listSkill[i]?.skillCore != null && listSkill[i].skillLevel > 0 &&
                listSkill[i].skillCore.skillType == SkillCore.SkillType.Active)
            {
                skill = listSkill[i];
                skillUI = listSkillUI[i];
                return true;
            }
        }

        skill = null;
        skillUI = null;
        return false;
    }

    public int TreePosition => treePos;

    public void GetSkill(int position)
    {
        currentSkill = position;

        SkillUIManager.instance.description.text = EnglishTextCatalog.GetSkillDescription(listSkill[position].skillCore);

        LoadUI(position);
    }

    public void GetTreePos(int pos)
    {
        SkillUIManager.instance.treeIndex = treePos;
    }

    private void LoadUI(int position)
    {
        SkillUIManager.instance.skillName.text = listSkill[position].skillCore.skillName + "(" + listSkill[position].skillLevel + ")";
        if (listSkill[position].skillLevel <= 0)
        {
            SkillUIManager.instance.upgradeOrUnlock.text = "Unlock";
        }
        else
        {
            SkillUIManager.instance.upgradeOrUnlock.text = "Upgrade";
        }


        SkillUIManager.instance.skillAction[0].SetActive(false);
        if (listSkill[position].skillLevel >= 3)
        {
            SkillUIManager.instance.upgradeOrUnlock.text = "Max Upgrade";
        }
        

        if (listSkill[position].skillCore.skillType == SkillCore.SkillType.Active && listSkill[position].skillLevel > 0)
        {
            SkillUIManager.instance.skillAction[1].SetActive(true);
            //SkillUIManager.instance.skillAction[2].SetActive(true);
        }
        else
        {

            SkillUIManager.instance.skillAction[1].SetActive(false);
            //SkillUIManager.instance.skillAction[2].SetActive(false);
        }
    }

    public GSkillCore SwapSkill()
    {
        return listSkill[currentSkill];
    }

    public SkillUI SwapSkillUI()
    {
        return listSkillUI[currentSkill];
    }
}
