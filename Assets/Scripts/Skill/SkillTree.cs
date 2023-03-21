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

public class SkillTree : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private int treePos;
    [SerializeField] private SkillTreeType treeType;
    [SerializeField] private List<SkillCore> listSkill;
    [SerializeField] private List<SkillUI> listSkillUI;
    private int currentSkill;

    private void Start()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnSkillUpgrade, (param) => OnSkillUpgrade());
    }

    private void OnSkillUpgrade()
    {
        if (SkillUIManager.instance.treeIndex == treePos)
        {
            if (listSkill[currentSkill].canUnlock && listSkill[currentSkill].skillLevel < 3)
            {
                listSkill[currentSkill].skillLevel++;
                if (listSkill[currentSkill].skillType == SkillCore.SkillType.Passive && listSkill[currentSkill].skillLevel == 1)
                {
                    PassiveSkillHolder.instance.AddPassiveSkill(listSkill[currentSkill], listSkillUI[currentSkill]);
                }

                LoadUI(currentSkill);
                if (listSkill[currentSkill].skillLevel >= 3)
                {
                    listSkill[currentSkill].skillToUnlock.canUnlock = true;
                }
            }
            else
            {
                this.PostEvent(EventID.OnSkillUpgradeFailed);
            }
        }
    }

    public void GetSkill(int position)
    {
        currentSkill = position;

        SkillUIManager.instance.description.text = listSkill[position].skillDescription;

        LoadUI(position);
    }

    public void GetTreePos(int pos)
    {
        SkillUIManager.instance.treeIndex = treePos;
    }

    private void LoadUI(int position)
    {
        SkillUIManager.instance.skillName.text = listSkill[position].skillName + "(" + listSkill[position].skillLevel + ")";
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
        

        if (listSkill[position].skillType == SkillCore.SkillType.Active && listSkill[position].skillLevel > 0)
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

    public SkillCore SwapSkill()
    {
        return listSkill[currentSkill];
    }

    public SkillUI SwapSkillUI()
    {
        return listSkillUI[currentSkill];
    }
}
