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

    public void Action()
    {
        skillCore.Action(skillLevel);
    }
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
        //listSkill[0].canUnlock = true;
        Debug.Log("Check skill: " + listSkill[0].skillCore.canUnlock);
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnSkillUpgrade, (param) => OnSkillUpgrade());
        this.RegisterListener(EventID.OnPlayerEnterGate, (param) => OnPlayerEnterGate());
    }

    private void OnPlayerEnterGate()
    {
        for(int i = 0; i < listSkill.Count; i++)
        {
            SaveData.SaveSingleData("gskill" + i+"|"+treePos, listSkill[i].skillLevel);
        }
    }

    private void OnSkillUpgrade()
    {
        if (SkillUIManager.instance.treeIndex == treePos)
        {
            if (listSkill[currentSkill].canUnlock && listSkill[currentSkill].skillLevel < 3)
            {
                listSkill[currentSkill].skillLevel++;
                if (listSkill[currentSkill].skillCore.skillType == SkillCore.SkillType.Passive && listSkill[currentSkill].skillLevel == 1)
                {
                    PassiveSkillHolder.instance.AddPassiveSkill(listSkill[currentSkill], listSkillUI[currentSkill]);
                }

                LoadUI(currentSkill);
                if (listSkill[currentSkill].skillLevel >= 3 && currentSkill<4)
                {
                    listSkill[currentSkill+1].canUnlock = true;
                }
            }
            else
            {
                this.PostEvent(EventID.OnSkillUpgradeFailed);
            }
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
                if (listSkill[i].skillCore.skillType == SkillCore.SkillType.Passive)
                {
                    PassiveSkillHolder.instance.AddPassiveSkill(listSkill[i], listSkillUI[i]);
                }
            }
            if (listSkill[i].skillLevel >= 3 && i < listSkill.Count - 1)
            {
                listSkill[i + 1].canUnlock = true;
            }
        }
    }

    public void GetSkill(int position)
    {
        currentSkill = position;

        SkillUIManager.instance.description.text = listSkill[position].skillCore.skillDescription;

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


        if (listSkill[position].skillLevel < 3)
        {
            SkillUIManager.instance.skillAction[0].SetActive(true);
        }
        else
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
