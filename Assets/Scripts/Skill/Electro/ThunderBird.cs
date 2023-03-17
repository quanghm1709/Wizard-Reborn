using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Electro/ThunderBird", fileName = "ThunderBird")]
public class ThunderBird : SkillCore
{
    public override void Action()
    {
        if (player.enemyInRange.Count > 0 && player.currentMp >= mpUse[skillLevel - 1])
        {           
            {
                int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

                GameObject g = Instantiate(skillAnim, player.transform.position, Quaternion.identity);
                g.GetComponent<SkillProjectile>().target = player.enemyInRange[enemyToDamage];
                g.GetComponent<SkillProjectile>().damage = atk[skillLevel - 1] * player.currentAtk;
            }

            player.currentMp -= mpUse[skillLevel - 1];
        }
    }
}
