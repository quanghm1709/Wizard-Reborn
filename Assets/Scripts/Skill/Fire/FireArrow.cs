using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/Fire/FireArrow", fileName = "FireArrow")]
public class FireArrow : SkillCore
{
    public override void Action(int level)
    {
        if (player.enemyInRange.Count > 0)
        {
            AudioManager.instance.audioSource[3].Play();
            float canShoot = Random.Range(0f, 1f);
            if (canShoot < .5f + (.25f * level))
            {
                int enemyToDamage = Random.Range(0, player.enemyInRange.Count);

                GameObject g = Instantiate(skillAnim, player.transform.position, Quaternion.identity);
                g.GetComponent<SkillProjectile>().target = player.enemyInRange[enemyToDamage];
                g.GetComponent<SkillProjectile>().damage = atk[level - 1] * player.currentAtk;
            }
        }
    }
}
