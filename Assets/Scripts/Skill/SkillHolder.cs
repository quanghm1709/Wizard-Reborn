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
                            CameraController.instance.Shake();
                            currentSkill[i].Action();
                            skillState[i] = SkillState.Cooldown;
                            cdTime[i] = currentSkill[i].cdTime[currentSkill[i].skillLevel - 1];
                        }
                        break;
                    case SkillState.Cooldown:
                        if (cdTime[i] > 0)
                        {
                            cdTime[i] -= Time.deltaTime;
                            SkillUIManager.instance.skillCD[i].fillAmount = cdTime[i] / currentSkill[i].cdTime[currentSkill[i].skillLevel - 1];
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
        int treeIndex = SkillUIManager.instance.treeIndex;
        foreach (SkillCore s in currentSkill)
        {
            if (s != null)
            {
                if (s.skillName == SkillUIManager.instance.skillTrees[treeIndex].SwapSkill().skillName)
                {
                    return;
                }
            }
        }
        currentSkill[param] = SkillUIManager.instance.skillTrees[treeIndex].SwapSkill();
        currentSkill[param].Init(GetComponent<PlayerController>());
        currentSkillUI[param] = SkillUIManager.instance.skillTrees[treeIndex].SwapSkillUI();
        cdTime[param] = currentSkill[param].cdTime[currentSkill[param].skillLevel-1];
        skillState[param] = SkillState.Ready;

        SkillUIManager.instance.activeSkillBtn[param].sprite = currentSkillUI[param].skillIcon;
        SkillUIManager.instance.skillCD[param].gameObject.SetActive(true);
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
