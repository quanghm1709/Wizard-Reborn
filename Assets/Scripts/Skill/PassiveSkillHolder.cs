using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillHolder : MonoBehaviour
{
    public static PassiveSkillHolder instance;

    [SerializeField] private List<float> cdTime;
    [SerializeField] private List<SkillCore> currentSkill;
    [SerializeField] private List<SkillUI> currentSkillUI;
    [SerializeField] private List<SkillState> skillState;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < currentSkill.Count; i++)
        {
            if (currentSkill[i] != null)
            {
                if(skillState[i] == SkillState.None)
                {
                    skillState[i] = SkillState.Ready;
                }

                switch (skillState[i])
                {
                    case SkillState.Ready:
                        currentSkill[i].Action();
                        skillState[i] = SkillState.Cooldown;
                        cdTime[i] = currentSkill[i].cdTime[currentSkill[i].skillLevel - 1];
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

    public void AddPassiveSkill(SkillCore skillCore, SkillUI skillUI)
    {
        skillCore.Init(GetComponent<PlayerController>());
        currentSkill.Add(skillCore);
        currentSkillUI.Add(skillUI);

        cdTime.Add(skillCore.cdTime[skillCore.skillLevel-1]);
        skillState.Add( new SkillState());
    }
}
