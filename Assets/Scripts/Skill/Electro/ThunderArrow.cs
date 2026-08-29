using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/Electro/ThunderArrow", fileName = "ThunderArrow")]
public class ThunderArrow : SkillCore
{
    public override bool Action(int level)
    {
        player.enemyInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        if (player.enemyInRange.Count > 0)
        {
            AudioManager.instance.audioSource[1].Play();
            float canShoot = Random.Range(0f, 1f);
            if(canShoot < .25f + (.25f * level))
            {
                int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

                GameObject g = Instantiate(skillAnim, player.transform.position, Quaternion.identity);
                SkillProjectile projectile = g.GetComponent<SkillProjectile>();
                projectile.target = player.enemyInRange[enemyToDamage];
                projectile.damage = ScaleDamage(atk[level - 1] * player.currentAtk);
                projectile.element = ElementType.Electro;
            }
            return true;
        }
        return false;
    }
}
