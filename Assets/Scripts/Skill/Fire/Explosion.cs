using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Fire/Explosion", fileName = "Explosion")]
public class Explosion : SkillCore
{
    public override bool Action(int level)
    {
        player.enemyInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        if (CanCast(level) && player.enemyInRange.Count > 0)
        {
            AudioManager.instance.audioSource[5].Play();
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);
            CameraController.instance.Shake(10f, .01f,.5f);
            Instantiate(skillAnim, player.enemyInRange[enemyToDamage].transform.position, Quaternion.identity);
            Vector3 targetPosition = player.enemyInRange[enemyToDamage].transform.position;
            player.StartCoroutine(ExplosionDmg(targetPosition, level));
            player.currentMp -= mpUse[level - 1];
            return true;
        }
        return false;
    }

    private IEnumerator ExplosionDmg(Vector3 targetPosition, int level)
    {
        yield return new WaitForSeconds(.3f);
        Collider2D[] hit = Physics2D.OverlapCircleAll(targetPosition, dmgRange, layerToDamage);
        int damage = ScaleDamage(atk[level - 1]);
        foreach (Collider2D c in hit)
        {
            IDamage damageable = c.GetComponent<IDamage>();
            damageable?.TakeDamage(damage, damage, 0);
            ApplyElement(c, ElementType.Fire, damage);
        }
    }
}
