using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCore : ScriptableObject
{
    public enum SkillType
    {
        None,
        Active,
        Passive,
    }

    public int skillLevel;
    public string skillName;
    public string skillDescription;
    public bool canUnlock;
    public SkillType skillType;
    public float[] cdTime;
    public float dmgRange;
    public float[] atk;
    public float[] mpUse;
    public LayerMask layerToDamage;

    public SkillCore skillToUnlock;
    public GameObject skillAnim;

    [SerializeField] private string skillId;
    public string SkillId => string.IsNullOrWhiteSpace(skillId) ? name : skillId;

    protected PlayerController player = null;
    public void Init(PlayerController playerController)
    {
        player = playerController;
    }

    public virtual bool CanCast(int level)
    {
        if (player == null || level <= 0 || level > cdTime.Length)
        {
            return false;
        }

        if (skillType == SkillType.Active && level <= mpUse.Length && player.currentMp < mpUse[level - 1])
        {
            return false;
        }

        return true;
    }

    public virtual bool Action(int skillLevel) { return false; }
}
