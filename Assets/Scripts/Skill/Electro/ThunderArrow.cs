using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/Electro/ThunderArrow", fileName = "ThunderArrow")]
public class ThunderArrow : SkillCore
{
    public override void Action()
    {
        if (player.enemyInRange.Count > 0)
        {
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

            GameObject g = Instantiate(skillAnim, player.transform.position, Quaternion.identity);
            g.GetComponent<SkillProjectile>().target = player.enemyInRange[enemyToDamage];
        }
    }
}
