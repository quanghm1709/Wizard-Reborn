using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Fire/Ember", fileName = "Ember")]
public class Ember : SkillCore
{
    public override bool Action(int level)
    {
        if(CanCast(level))
        {
            AudioManager.instance.audioSource[4].Play();
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
            int damage = ScaleDamage(atk[level - 1]);

            foreach (Collider2D c in hit)
            {
                IDamage damageable = c.GetComponent<IDamage>();
                damageable?.TakeDamage(damage, damage, 0);
                ApplyElement(c, ElementType.Fire, damage);
            }
            player.currentMp -= mpUse[level - 1];
            return true;
        }
        return false;
    }
}
