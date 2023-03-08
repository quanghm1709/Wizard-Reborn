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

    public void GetSkill(int position)
    {
        SkillUIManager.instance.skillName.text = listSkill[position].skillName;
        SkillUIManager.instance.description.text = listSkill[position].skillDescription;
    }
}
