using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager instance;

    public Image[] activeSkillBtn;
    public Text skillName;
    public Text description;
    public GameObject[] skillAction;//0 - upgrade, 1 - Set Active, 2 - set auto
    public Text upgradeOrUnlock;

    private void Start()
    {
        instance = this;
    }

    public void UpgradeSkill()
    {
        this.PostEvent(EventID.OnSkillUpgradeClick);
    }
}
