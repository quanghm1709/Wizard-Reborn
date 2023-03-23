using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Fire/Ember", fileName = "Ember")]
public class Ember : SkillCore
{
    public override void Action(int level)
    {
        if(player.currentMp >= mpUse[level - 1])
        {
            Vector3 pos;
            int scale;
            if (player.isFacingRight)
            {
                pos = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);
                scale = 2;
            }
            else
            {
                pos = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
                scale = -2;
            }

            GameObject skill = Instantiate(skillAnim, pos, Quaternion.identity);
            skill.transform.localScale = new Vector3(scale, 2, 1);
            Collider2D[] hit = Physics2D.OverlapCircleAll(pos, dmgRange, layerToDamage);

            foreach (Collider2D c in hit)
            {
                c.GetComponent<IDamage>().TakeDamage((int)atk[level - 1], (int)atk[level - 1], 0);
            }
            player.currentMp -= mpUse[level - 1];
        }
        
    }
}
