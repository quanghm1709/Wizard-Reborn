using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCore : ScriptableObject
{
    public int skillLevel;
    public string skillName;
    public float cdTime;
    public float dmgRange;
    public float[] atk;
    public SkillCore skillToUnlock;
    public GameObject skillAnim;

    protected PlayerController player = null;
    public void Init(PlayerController playerController)
    {
        player = playerController;
    }

    public virtual void Action() { }
}
