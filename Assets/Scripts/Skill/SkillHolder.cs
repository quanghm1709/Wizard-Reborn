using System;
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
    [SerializeField] private List<float> cdTime;
    [SerializeField] private List<SkillCore> currentSkill;
    [SerializeField] private List<SkillUI> currentSkillUI;
    [SerializeField] private List<SkillState> skillState;

    private void Start()
    {
        RegisterEvent();
        AddSkillToUI();
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if(currentSkill[i] != null)
            {
                currentSkill[i].Init(GetComponent<PlayerController>());
                skillState[i] = SkillState.Ready;
            }

        }           
    }

    private void Update()
    {
        for(int i = 0; i < currentSkill.Count; i++)
        {
            if (currentSkill[i] != null)
            {
                switch (skillState[i])
                {
                    case SkillState.Ready:
                        if (CrossPlatformInputManager.GetButtonDown("Skill " + i))
                        {
                            currentSkill[i].Action();
                            skillState[i] = SkillState.Cooldown;
                            cdTime[i] = currentSkill[i].cdTime[currentSkill[i].skillLevel - 1];
                        }
                        break;
                    case SkillState.Cooldown:
                        if (cdTime[i] > 0)
                        {
                            cdTime[i] -= Time.deltaTime;
                        }
                        else
                        {
                            skillState[i] = SkillState.Ready;
                        }
                        break;
                }
            }
        }
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnSwapSkill, (param) => OnSwapSkill((int)param));
    }

    private void OnSwapSkill(int param)
    {
        foreach(SkillCore s in currentSkill)
        {
            if (s != null)
            {
                if (s.skillName == SkillUIManager.instance.skillTrees[0].SwapSkill().skillName)
                {
                    return;
                }
            }
        }
        currentSkill[param] = SkillUIManager.instance.skillTrees[0].SwapSkill();
        currentSkill[param].Init(GetComponent<PlayerController>());
        currentSkillUI[param] = SkillUIManager.instance.skillTrees[0].SwapSkillUI();
        cdTime[param] = currentSkill[param].cdTime[currentSkill[param].skillLevel-1];
        skillState[param] = SkillState.Ready;

        SkillUIManager.instance.activeSkillBtn[param].sprite = currentSkillUI[param].skillIcon;
    }

    private void AddSkillToUI()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (currentSkill[i] != null)
            {
                SkillUIManager.instance.activeSkillBtn[i].sprite = currentSkillUI[i].skillIcon;
            }
        }
    }
}
