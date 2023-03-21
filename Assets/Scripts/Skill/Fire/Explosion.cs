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
            int enemyToDamage = Random.Range(0, player.enemyInRange.Count);
            CameraController.instance.Shake(10f, .01f,.5f);
            Instantiate(skillAnim, player.enemyInRange[enemyToDamage].transform.position, Quaternion.identity);
            ExplosionDmg(enemyToDamage);
            player.currentMp -= mpUse[skillLevel - 1];
        }
    }

    private IEnumerator ExplosionDmg(int enemyToDamage)
    {
        yield return new WaitForSeconds(.3f);
        Collider2D[] hit = Physics2D.OverlapCircleAll(player.enemyInRange[enemyToDamage].transform.position, dmgRange, layerToDamage);
        foreach (Collider2D c in hit)
        {
            c.GetComponent<IDamage>().TakeDamage((int)atk[0], (int)atk[0], 0);
        }
    }
}
