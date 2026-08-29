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
        EnsureSlotCapacity();
    }

    private void Start()
    {
        RegisterEvent();
        AddSkillToUI();
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if(IsValidSkill(i))
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
            if (IsValidSkill(i))
            {
                switch (skillState[i])
                {
                    case SkillState.Ready:
                        if (CrossPlatformInputManager.GetButtonDown("Skill " + i) && currentSkill[i].skillCore.CanCast(currentSkill[i].skillLevel))
                        {
                            if (currentSkill[i].Action())
                            {
                                skillState[i] = SkillState.Cooldown;
                                cdTime[i] = currentSkill[i].skillCore.cdTime[currentSkill[i].skillLevel - 1];
                            }
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
        this.RegisterListener(EventID.OnSwapSkill, HandleSwapSkill);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.OnSwapSkill, HandleSwapSkill);
    }

    private void HandleSwapSkill(object param)
    {
        if (param is int slot)
        {
            OnSwapSkill(slot);
        }
    }

    private void OnSwapSkill(int param)
    {
        int treeIndex = SkillUIManager.instance.treeIndex;
        if (treeIndex < 0 || treeIndex >= SkillUIManager.instance.skillTrees.Count)
        {
            return;
        }

        foreach (GSkillCore s in currentSkill)
        {
            if (s != null && s.skillCore != null)
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
        if (treeIndex < 0 || treeIndex >= SkillUIManager.instance.skillTrees.Count)
        {
            return;
        }

        SetSkill(param, SkillUIManager.instance.skillTrees[treeIndex].SwapSkill(), SkillUIManager.instance.skillTrees[treeIndex].SwapSkillUI());

        SaveData.SaveSingleData("pos" + param + "|"+treeIndex, param);
        //SaveData.SaveSingleData("treeindex" + param + "|"+treeIndex, param);
    }

    public void LoadData()
    {
        for(int i = 0; i < currentSkill.Count; i++)
        {
            for(int j = 0; j < SkillUIManager.instance.skillTrees.Count; j++)
            {
                if (SaveData.HasSingleKey("pos" + i + "|" + j))
                {
                    SkillTree tree = SkillUIManager.instance.skillTrees[j];
                    if (tree.TryGetFirstUnlockedActive(out GSkillCore skill, out SkillUI skillUI))
                    {
                        SetSkill(i, skill, skillUI);
                    }
                }
            }          
        }
    }

    public List<EquippedSkillSaveData> CaptureSaveData()
    {
        List<EquippedSkillSaveData> result = new List<EquippedSkillSaveData>();
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (IsValidSkill(i))
            {
                result.Add(new EquippedSkillSaveData { slot = i, skillId = currentSkill[i].skillCore.SkillId });
            }
        }
        return result;
    }

    public void ApplySaveData(List<EquippedSkillSaveData> equippedSkills, List<SkillTree> trees)
    {
        ClearEquippedSkills();
        if (equippedSkills == null)
        {
            return;
        }

        foreach (EquippedSkillSaveData equipped in equippedSkills)
        {
            if (equipped == null || equipped.slot < 0 || equipped.slot >= currentSkill.Count)
            {
                continue;
            }

            foreach (SkillTree tree in trees)
            {
                if (tree.TryGetSkill(equipped.skillId, out GSkillCore skill, out SkillUI skillUI) && skill.skillLevel > 0)
                {
                    SetSkill(equipped.slot, skill, skillUI);
                    break;
                }
            }
        }
    }

    private void SetSkill(int slot, GSkillCore skill, SkillUI skillUI)
    {
        if (slot < 0 || slot >= currentSkill.Count || skill?.skillCore == null || skillUI == null || skill.skillLevel <= 0)
        {
            return;
        }

        currentSkill[slot] = skill;
        currentSkillUI[slot] = skillUI;
        skill.skillCore.Init(GetComponent<PlayerController>());
        cdTime[slot] = 0f;
        skillState[slot] = SkillState.Ready;

        if (SkillUIManager.instance != null)
        {
            SkillUIManager.instance.activeSkillBtn[slot].sprite = skillUI.skillIcon;
            SkillUIManager.instance.skillCD[slot].gameObject.SetActive(true);
        }
    }

    private void ClearEquippedSkills()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            currentSkill[i] = null;
            currentSkillUI[i] = null;
            cdTime[i] = 0f;
            skillState[i] = SkillState.None;
            if (SkillUIManager.instance != null && i < SkillUIManager.instance.skillCD.Count)
            {
                SkillUIManager.instance.skillCD[i].gameObject.SetActive(false);
            }
        }
    }
    private void AddSkillToUI()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (IsValidSkill(i))
            {
                SkillUIManager.instance.activeSkillBtn[i].sprite = currentSkillUI[i].skillIcon;
            }
        }
    }

    private bool IsValidSkill(int index)
    {
        return index >= 0 && index < currentSkill.Count && currentSkill[index] != null &&
               currentSkill[index].skillCore != null && currentSkill[index].skillLevel > 0;
    }

    private void EnsureSlotCapacity()
    {
        if (currentSkill == null) currentSkill = new List<GSkillCore>();
        if (currentSkillUI == null) currentSkillUI = new List<SkillUI>();
        if (cdTime == null) cdTime = new List<float>();
        if (skillState == null) skillState = new List<SkillState>();

        int slotCount = Mathf.Max(4, currentSkill.Count);
        while (currentSkill.Count < slotCount) currentSkill.Add(null);
        while (currentSkillUI.Count < slotCount) currentSkillUI.Add(null);
        while (cdTime.Count < slotCount) cdTime.Add(0f);
        while (skillState.Count < slotCount) skillState.Add(SkillState.None);
    }
}
