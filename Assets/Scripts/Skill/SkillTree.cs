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
        if (listSkill[currentSkill].canUnlock && listSkill[currentSkill].skillLevel < 3)
        {
            if(listSkill[currentSkill].skillType == SkillCore.SkillType.Passive && listSkill[currentSkill].skillLevel == 0)
            {
                PassiveSkillHolder.instance.AddPassiveSkill(listSkill[currentSkill], listSkillUI[currentSkill]);
            }
            listSkill[currentSkill].skillLevel++;
            LoadUI(currentSkill);
            if(listSkill[currentSkill].skillLevel >= 3)
            {
                listSkill[currentSkill].skillToUnlock.canUnlock = true;
            }
        }
        else
        {
            this.PostEvent(EventID.OnSkillUpgradeFailed);
        }
        
    }

    public void GetSkill(int position)
    {
        currentSkill = position;
        
        SkillUIManager.instance.description.text = listSkill[position].skillDescription;

        LoadUI(position);
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
