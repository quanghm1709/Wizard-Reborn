using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillState
{
    None,
    Active,
    Cooldown,
}

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private float cdTime;
    [SerializeField] private SkillCore currentSkill;
}
