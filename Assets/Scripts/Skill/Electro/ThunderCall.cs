using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/Electro/ThunderCall", fileName = "ThunderCall")]
public class ThunderCall : SkillCore
{
    public override bool Action(int level)
    {
        player.enemyInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        if (player.enemyInRange.Count > 0)
        {
            AudioManager.instance.audioSource[2].Play();
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

            Instantiate(skillAnim, player.enemyInRange[enemyToDamage].transform.position, Quaternion.identity);
            Collider2D[] hit = Physics2D.OverlapCircleAll(player.enemyInRange[enemyToDamage].transform.position, dmgRange, layerToDamage);
            int damage = ScaleDamage(atk[level - 1]);
            foreach (Collider2D c in hit)
            {
                IDamage damageable = c.GetComponent<IDamage>();
                damageable?.TakeDamage(damage, damage, 0);
                ApplyElement(c, ElementType.Electro, damage);
            }
            return true;
        }
        return false;
    }
}
