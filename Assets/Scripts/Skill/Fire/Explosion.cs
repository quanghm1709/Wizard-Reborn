using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Fire/Explosion", fileName = "Explosion")]
public class Explosion : SkillCore
{
    public override void Action()
    {
        if (player.enemyInRange.Count > 0 && player.currentMp >= mpUse[skillLevel - 1])
        {

            player.currentMp -= mpUse[skillLevel - 1];
        }
    }
}
