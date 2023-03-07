using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager instance;

    public Image[] activeSkillBtn;

    private void Start()
    {
        instance = this;
    }
}
