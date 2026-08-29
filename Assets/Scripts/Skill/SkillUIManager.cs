using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager instance;

    public Image[] activeSkillBtn;
    public List<Image> skillCD;
    public Text skillPoint;
    public Text skillName;
    public Text description;
    public GameObject[] skillAction;//0 - upgrade, 1 - Set Active
    public Text upgradeOrUnlock;
    public GameObject swapSkill;

    public List<SkillTree> skillTrees;
    public int treeIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RegisterEvent();
        if (skillPoint != null)
        {
            skillPoint.gameObject.SetActive(false);
        }
        if (skillAction != null && skillAction.Length > 0 && skillAction[0] != null)
        {
            skillAction[0].SetActive(false);
        }
        //foreach(SkillTree tree in skillTrees)
        //{
        //    tree.LoadSkill();
        //    treeIndex++;
        //}
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnOpenMenu, HandleOpenMenu);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.OnOpenMenu, HandleOpenMenu);
    }

    private void HandleOpenMenu(object param)
    {
        ResetUI();
    }
    public void UpgradeSkill()
    {
        // Skill upgrades are now selected from the level-up popup.
    }

    public void OpenSwapSkill()
    {
        if (swapSkill.activeInHierarchy)
        {
            swapSkill.SetActive(false);
        }
        else
        {
            swapSkill.SetActive(true);
        }
    }

    public void SwapSkill(int pos)
    {
        this.PostEvent(EventID.OnSwapSkill, pos);
        swapSkill.SetActive(false);
    }

    public void ResetUI()
    {
        skillName.text = "";
        description.text = "";

        foreach(GameObject g in skillAction)
        {
            g.SetActive(false);
        }
    }
}
