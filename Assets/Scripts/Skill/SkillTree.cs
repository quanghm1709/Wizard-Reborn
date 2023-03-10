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
        listSkill[currentSkill].skillLevel++;
    }

    public void GetSkill(int position)
    {
        currentSkill = position;
        SkillUIManager.instance.skillName.text = listSkill[position].skillName;
        SkillUIManager.instance.description.text = listSkill[position].skillDescription;

        if(listSkill[position].skillType == SkillCore.SkillType.Active)
        {
            foreach(GameObject g in SkillUIManager.instance.skillAction)
            {
                g.SetActive(true);
            }
        }
        else
        {
            SkillUIManager.instance.skillAction[0].SetActive(true);
            SkillUIManager.instance.skillAction[1].SetActive(false);
            SkillUIManager.instance.skillAction[2].SetActive(false);
        }
    }
}
