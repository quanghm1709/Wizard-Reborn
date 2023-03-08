using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Electro/ThunderArrow", fileName = "ThunderArrow")]
public class ThunderArrow : SkillCore
{
    public override void Action()
    {
        if (player.enemyInRange.Count > 0)
        {
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

            Instantiate(skillAnim, player.enemyInRange[enemyToDamage].transform.position, Quaternion.identity);
            Collider2D[] hit = Physics2D.OverlapCircleAll(player.enemyInRange[enemyToDamage].transform.position, dmgRange, layerToDamage);
            foreach (Collider2D c in hit)
            {
                c.GetComponent<IDamage>().TakeDamage((int)atk[0], (int)atk[0], 0);
            }
        }
    }
}
