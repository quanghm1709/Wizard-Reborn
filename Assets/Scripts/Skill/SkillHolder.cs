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

    [SerializeField] private float cdTime2;
    [SerializeField] private SkillCore currentSkill2;
    [SerializeField] private SkillUI currentSkillUI2;
    [SerializeField] private SkillState skillState2 = SkillState.Ready;

    private void Start()
    {
        AddSkillToUI();
        currentSkill.Init(GetComponent<PlayerController>());
        currentSkill2.Init(GetComponent<PlayerController>());
    }

    private void Update()
    {
        switch (skillState)
        {
            case SkillState.Ready:
                if(CrossPlatformInputManager.GetButtonDown("Skill 1")){
                    currentSkill.Action();
                    skillState = SkillState.Cooldown;
                    cdTime = currentSkill.cdTime[currentSkill.skillLevel-1];
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

        switch (skillState2)
        {
            case SkillState.Ready:
                if (CrossPlatformInputManager.GetButtonDown("Skill 2"))
                {
                    currentSkill2.Action();
                    skillState2 = SkillState.Cooldown;
                    cdTime2 = currentSkill2.cdTime[currentSkill2.skillLevel-1];
                }
                break;
            case SkillState.Cooldown:
                if (cdTime2 > 0)
                {
                    cdTime2 -= Time.deltaTime;
                }
                else
                {
                    skillState2 = SkillState.Ready;
                }
                break;
        }
    }

    private void AddSkillToUI()
    {
        SkillUIManager.instance.activeSkillBtn[0].sprite = currentSkillUI.skillIcon;
        SkillUIManager.instance.activeSkillBtn[1].sprite = currentSkillUI2.skillIcon;
    }
}
