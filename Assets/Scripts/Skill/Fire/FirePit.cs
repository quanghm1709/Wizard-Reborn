using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/Fire/FirePit", fileName = "FirePit")]
public class FirePit : SkillCore
{
    public override bool Action(int level)
    {
        player.enemyInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        if (CanCast(level) && player.enemyInRange.Count > 0 && level <= mpUse.Length && player.currentMp >= mpUse[level - 1])
        {
            AudioManager.instance.audioSource[3].Play();
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);
            CameraController.instance.Shake(10f, .01f, .1f);
            Instantiate(skillAnim, player.enemyInRange[enemyToDamage].transform.position, Quaternion.identity);
            Collider2D[] hit = Physics2D.OverlapCircleAll(player.enemyInRange[enemyToDamage].transform.position, dmgRange, layerToDamage);
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
