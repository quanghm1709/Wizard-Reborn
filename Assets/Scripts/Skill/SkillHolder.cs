using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public enum SkillState
{
    None,
    Ready,
    Cooldown,
}

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private float cdTime;
    [SerializeField] private SkillCore currentSkill;
    [SerializeField] private SkillUI currentSkillUI;
    [SerializeField] private SkillState skillState = SkillState.Ready;
    
    private void Start()
    {
        AddSkillToUI();
        currentSkill.Init(GetComponent<PlayerController>());
    }

    private void Update()
    {
        switch (skillState)
        {
            case SkillState.Ready:
                if(CrossPlatformInputManager.GetButtonDown("Skill 1")){
                    currentSkill.Action();
                    skillState = SkillState.Cooldown;
                    cdTime = currentSkill.cdTime;
                }
                break;
            case SkillState.Cooldown:
                if (cdTime > 0)
                {
                    cdTime -= Time.deltaTime;
                }
                else
                {
                    skillState = SkillState.Ready;
                }
                break;
        }
    }

    private void AddSkillToUI()
    {
        SkillUIManager.instance.activeSkillBtn[0].sprite = currentSkillUI.skillIcon;
    }
}
