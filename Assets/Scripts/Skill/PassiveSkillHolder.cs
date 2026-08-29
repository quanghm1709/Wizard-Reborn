using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillHolder : MonoBehaviour
{
    public static PassiveSkillHolder instance;

    [SerializeField] private List<float> cdTime;
    [SerializeField] private List<GSkillCore> currentSkill;
    [SerializeField] private List<SkillUI> currentSkillUI;
    [SerializeField] private List<SkillState> skillState;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (currentSkill[i] != null && currentSkill[i].skillCore != null && currentSkill[i].skillLevel > 0)
            {
                if(skillState[i] == SkillState.None)
                {
                    skillState[i] = SkillState.Ready;
                }

                switch (skillState[i])
                {
                    case SkillState.Ready:
                        if (currentSkill[i].Action())
                        {
                            skillState[i] = SkillState.Cooldown;
                            cdTime[i] = currentSkill[i].skillCore.cdTime[currentSkill[i].skillLevel - 1];
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

    public void AddPassiveSkill(GSkillCore skillCore, SkillUI skillUI)
    {
        if (skillCore == null || skillCore.skillCore == null || skillCore.skillLevel <= 0 || currentSkill.Contains(skillCore))
        {
            return;
        }

        skillCore.skillCore.Init(GetComponent<PlayerController>());
        currentSkill.Add(skillCore);
        currentSkillUI.Add(skillUI);

        cdTime.Add(skillCore.skillCore.cdTime[skillCore.skillLevel-1]);
        skillState.Add( new SkillState());
    }

    public void ClearSkills()
    {
        currentSkill.Clear();
        currentSkillUI.Clear();
        cdTime.Clear();
        skillState.Clear();
    }
}
