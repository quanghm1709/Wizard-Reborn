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
    public static SkillHolder instance;
    [SerializeField] private List<float> cdTime;
    [SerializeField] private List<GSkillCore> currentSkill;
    [SerializeField] private List<SkillUI> currentSkillUI;
    [SerializeField] private List<SkillState> skillState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RegisterEvent();
        AddSkillToUI();
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if(currentSkill[i].skillCore != null)
            {
                currentSkill[i].skillCore.Init(GetComponent<PlayerController>());
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
                            cdTime[i] = currentSkill[i].skillCore.cdTime[currentSkill[i].skillLevel - 1];
                        }
                        break;
                    case SkillState.Cooldown:
                        if (cdTime[i] > 0)
                        {
                            cdTime[i] -= Time.deltaTime;
                            SkillUIManager.instance.skillCD[i].fillAmount = cdTime[i] / currentSkill[i].skillCore.cdTime[currentSkill[i].skillLevel - 1];
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

        foreach (GSkillCore s in currentSkill)
        {
            if (s.skillCore != null)
            {
                if (s.skillCore.skillName == SkillUIManager.instance.skillTrees[treeIndex].SwapSkill().skillCore.skillName)
                {
                    return;
                }
            }
        }
        AddSkill(param, treeIndex);
    }

    private void AddSkill(int param, int treeIndex)
    {
        currentSkill[param] = SkillUIManager.instance.skillTrees[treeIndex].SwapSkill();
        currentSkill[param].skillCore.Init(GetComponent<PlayerController>());
        currentSkillUI[param] = SkillUIManager.instance.skillTrees[treeIndex].SwapSkillUI();
        cdTime[param] = currentSkill[param].skillCore.cdTime[currentSkill[param].skillLevel - 1];
        skillState[param] = SkillState.Ready;

        SkillUIManager.instance.activeSkillBtn[param].sprite = currentSkillUI[param].skillIcon;
        SkillUIManager.instance.skillCD[param].gameObject.SetActive(true);

        SaveData.SaveSingleData("pos" + param + "|"+treeIndex, param);
        //SaveData.SaveSingleData("treeindex" + param + "|"+treeIndex, param);
    }

    public void LoadData()
    {
        for(int i =0; i< 4; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                if (SaveData.HasSingleKey("pos" + i + "|" + j))
                {
                    AddSkill(i, j);
                }
            }          
        }
    }
    private void AddSkillToUI()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (currentSkill[i].skillCore != null)
            {
                SkillUIManager.instance.activeSkillBtn[i].sprite = currentSkillUI[i].skillIcon;
            }
        }
    }
}
